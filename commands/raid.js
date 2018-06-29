const moment = require("moment");
const sql = require("../util/sql");
const config = require("../config.js");

const STATUS = {
    AWAITING_DATE: 1,
    AWAITING_CLEAR_TYPE: 2,
    AWAITING_TEMPLATE: 3,
    AWAITING_PUBLISH: 4,
    PUBLISHED: 5,
    FULL: 6,
    COMPLETED: 10
};

const dateFormat = "YYYY-MM-DD";
const timeFormat = "HH:mm";

class RaidSetup {

    constructor(client, message, args) {
        this.client = client;
        this.message = message;
        this.args = args;
    }

    hasPermission() {
        const leaderRole = this.message.guild.roles.find("name", "Leader");
        if (!leaderRole) {
            console.log("The Leader role does not exist");
            return false;
        }

        if (!this.message.guild.roles.has(leaderRole.id)) {
            console.log("You can't use this command.");
            return false;
        }

        return true;
    }

    async getRaid(userId) {
        let [rows] = await
            sql.execute('SELECT * FROM raid WHERE status != ? AND created_by = ? LIMIT 1', [STATUS.COMPLETED, userId]);

        if (rows.length) {
            return rows[0];
        }
        return this.createRaid(userId)
    }

    async createRaid(userId) {
        let [result] = await
            sql.execute('INSERT INTO raid (created_by, status) VALUES (?, ?)', [userId, STATUS.AWAITING_DATE]);

        let squadInserts = [];

        for (let i = 1; i <= 10; i++) {
            squadInserts.push(sql.execute('INSERT INTO raid_squad (raid_id, spot) VALUES (?, ?)', [result.insertId, i]));
        }
        await
            Promise.all(squadInserts);

        return this.getRaid(userId);
    }

    async setupRaid(raid) {
        switch (raid.status) {
            case STATUS.AWAITING_DATE:
                await this.dateSetup(raid);
                break;
            case STATUS.AWAITING_CLEAR_TYPE:
                await this.clearTypeSetup(raid);
                break;
            case STATUS.AWAITING_TEMPLATE:
                await this.roleTemplateSetup(raid);
                break;
            case STATUS.AWAITING_PUBLISH:
                await this.raidPublish(raid);
                break;
             case STATUS.PUBLISHED:
                await this.raidPublish(raid);
                break;
        }
    }

    async dateSetup(raid) {
        const filter = m => m.member === this.message.member;

        this.message.reply("For when shall we schedule the raid?");

        let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 15000});

        if (!collected.size) {
            this.message.reply("No response received, please use the raid command to continue setting up when you are ready");
            return;
        }

        let message = collected.first();

        let parts = message.content.split("@").map(v => v.trim());

        try {
            let date = moment(parts[0]);

            if (!date.isValid()) {
                throw new Error("Unrecognized date format");
            }

            let time;
            if (parts[1]) {
                time = moment(parts[1], timeFormat);
                if (!time.isValid()) {
                    throw new Error("Unrecognized time format");
                }
            } else {
                time = moment("18:00", timeFormat);
            }

            await sql.execute(`UPDATE raid 
                    SET 
                        scheduled_for = ?,
                        status = ? 
                    WHERE
                        id = ?`, [`${date.format(dateFormat)} ${time.format(timeFormat)}`, STATUS.AWAITING_CLEAR_TYPE, raid.id]);

            raid = await this.getRaid(this.message.member.id);
        } catch (e) {
            this.message.reply("Something went wrong, likely unrecognized date/time format");
            console.error(e);
        }
        await this.setupRaid(raid);
    }

    // TODO: Add error handling and transaction encapsulation to prevent inconsistent data states
    async clearTypeSetup(raid) {

        // quick setup
        // @w1|w2|w3
        // @w1[1,2,3]|w3
        // @fc

        this.message.reply("What are we planning to do? [fullclear, wing #, boss, golem]");

        const filter = m => m.member === this.message.member;
        let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 30000});

        if (!collected.size) {
            this.message.reply("No response received, please use the raid command to continue setting up when you are ready");
            return;
        }

        try {
            let message = collected.first();

            let config = message.content.trim();

            if (config === "fc") {
                // Full Clear
                await sql.execute(`
                    INSERT INTO raid_clear_setup (raid_boss_id, raid_id)
                     SELECT
                        id,
                        ${raid.id}
                     FROM
                        raid_boss
                     `);
            } else {

                let wings = config.split("|");

                for (let i = 0; i < wings.length; i++) {
                    let wingConfig = wings[i];

                    if (wingConfig[0].toLowerCase() !== "w" || isNaN(Number(wingConfig[1]))) {
                        throw "Incorrect Syntax";
                    }

                    let wing = Number(wingConfig[1]);

                    // check if boss selection
                    if (wingConfig.indexOf("[") > -1) {
                        console.log(wingConfig);
                        console.log(wingConfig.match(/\[(.*)]/));
                        let match = wingConfig.match(/\[(.*)]/)[1];
                        if (match) {
                            let bossesInList = match.split(",").map(Number).filter(v => !isNaN(v)).join(",");

                            await sql.execute(`
                    INSERT INTO raid_clear_setup (raid_boss_id, raid_id)
                     SELECT
                        id,
                        ${raid.id}
                     FROM
                        raid_boss
                     WHERE
                        raid_wing_id = ?
                        AND number IN (${bossesInList})
                     `, [wing]);
                        }
                    } else {
                        await sql.execute(`
                    INSERT INTO raid_clear_setup (raid_boss_id, raid_id)
                     SELECT
                        id,
                        ${raid.id}
                     FROM
                        raid_boss
                     WHERE
                        raid_wing_id = ?
                     `, [wing]);
                    }
                }
            }

            await sql.execute(`UPDATE raid 
                    SET 
                        status = ? 
                    WHERE
                        id = ?`, [STATUS.AWAITING_TEMPLATE, raid.id]);

            raid = await this.getRaid(this.message.member.id);
        } catch (e) {
            this.message.reply("Something went wrong, are you sure you used the correct format?");
            console.error(e);
        }
        await this.setupRaid(raid);
    }

    async roleTemplateSetup(raid) {

        let [rows] = await
            sql.execute('SELECT * FROM raid_template');

        let validResponses = rows.map(row => row.id);
        let templates = rows.map(row => `${row.id}: ${row.name}`);
        templates.push("0: My Way - To Be Implemented");

        this.message.reply("Do you have a squad composition in mind?\n" + templates.join("\n"));

        const filter = m => m.member === this.message.member;
        let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 30000});

        if (!collected.size) {
            this.message.reply("No response received, please use the raid command to continue setting up when you are ready");
            return;
        }

        let message = collected.first();

        let choice = Number(message.content);

        if (validResponses.indexOf(choice) === -1) {
            this.message.reply("That is not a valid response. Please run the raid command to continue setting up.");
            return;
        }

        await
            sql.execute(`
            INSERT INTO raid_squad_restriction (raid_squad_id, raid_role_id, profession_id, guild_rank_id)
            SELECT
              rs.id,
              rtr.raid_role_id,
              rtr.profession_id,
              rtr.guild_rank_id
            FROM
              raid_template_restriction rtr
              INNER JOIN raid_squad rs ON rtr.spot = rs.spot
            WHERE
              rtr.raid_template_id = ?
              AND rs.raid_id = ?`, [choice, raid.id]);



        await sql.execute(`UPDATE raid 
                    SET 
                        status = ? 
                    WHERE
                        id = ?`, [STATUS.AWAITING_PUBLISH, raid.id]);

        raid = await this.getRaid(this.message.member.id);
        await this.setupRaid(raid);
    }

    async raidPublish(raid) {

        this.message.reply("Your raid is ready for publishing, here is a summary! Reply with 'done' to publish.");

        const filter = m => m.member === this.message.member;
        let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 30000});

        if (!collected.size) {
            this.message.reply("No response received, please use the raid command to continue setting up when you are ready");
            return;
        }

        let message = collected.first().content.trim().toLowerCase();

        switch (message)
        {
            case "done":
                await this.updateSchedule(raid.id, this.message.guild.channels.find('id', config.raidChannelId));
                await sql.execute(`UPDATE raid 
                        SET 
                            status = ? 
                        WHERE
                            id = ?`, [STATUS.PUBLISHED, raid.id]);
                this.message.reply("Published!");
                break;
            case "remove":
                let channel = this.message.guild.channels.find('id', config.raidChannelId);
                const fetched = await channel.fetchMessages({limit: 99});
                channel.bulkDelete(fetched);
                await sql.execute(`UPDATE raid 
                        SET 
                            status = ? 
                        WHERE
                            id = ?`, [STATUS.COMPLETED, raid.id]);
                this.message.reply("Raid marked as completed!");
                break;
            default:
                this.message.reply("Invalid response");
        }
    }

      async updateSchedule(raid, channel) {
        // temporary cleanup of old messages.
        const fetched = await channel.fetchMessages({limit: 99});
        channel.bulkDelete(fetched);

        let [clearTypeRows] = await
            sql.execute(`
            SELECT
              rw.name as 'WingName',
              rb.icon as 'BossIcon'
            FROM
              raid_clear_setup rcs
              INNER JOIN raid_boss rb on rcs.raid_boss_id = rb.id
              INNER JOIN raid_wing rw on rb.raid_wing_id = rw.id
            WHERE
              rcs.raid_id = ${raid}
            ORDER BY
              rw.id,
              rb.number`);


        let groupedClearTypes = {};
        for (let clearType of clearTypeRows) {
            if (!groupedClearTypes[clearType.WingName]) {
                groupedClearTypes[clearType.WingName] = {wing: clearType.WingName, data: []}
            }
            groupedClearTypes[clearType.WingName].data.push(clearType.BossIcon);

        }

        let clearFields = [];
        for (let index in groupedClearTypes) {
            let groupedClearType = groupedClearTypes[index];
            clearFields.push({
                name: `${groupedClearType.wing}:`,
                value: groupedClearType.data.join(" ")
            });
        }

        let [raidSquadRows] = await
            sql.execute(
                `SELECT
          *
        FROM
          raid_squad
        WHERE
          raid_id = ${raid}
        ORDER BY
           spot`);

        let [restrictionRows] = await
            sql.execute(
                `SELECT
          rsr.*
          , p.title as profession
          , p.icon as profession_icon
          , r.title as role
          , r.icon as role_icon
          , squad.spot
        FROM
          raid_squad_restriction rsr
          INNER JOIN raid_squad squad on rsr.raid_squad_id = squad.id
          LEFT JOIN profession p on rsr.profession_id = p.id
          LEFT JOIN raid_role r on rsr.raid_role_id = r.id
        WHERE
          squad.raid_id = ${raid}
        ORDER BY
            squad.spot`);

        let [ranks] = await sql.execute(`SELECT DISTINCT(guild_rank.rank)
            FROM guild_rank
            JOIN raid_squad_restriction ON raid_squad_restriction.guild_rank_id = guild_rank.id
            JOIN raid_squad ON raid_squad.id = raid_squad_restriction.raid_squad_id
            WHERE raid_squad.raid_id = ${raid}`);

        let (raidDescription, squadDescription) = "";

        for (let index in ranks)
        {
            raidDescription += `<@&${this.message.guild.roles.find("name", ranks[index])}> `;
        }

        let groupedRestrictionRows = {};
        for (let index in restrictionRows) {
            let row = restrictionRows[index];
            if (!groupedRestrictionRows[row.raid_squad_id]) {
                groupedRestrictionRows[row.raid_squad_id] = [];
            }
            groupedRestrictionRows[row.raid_squad_id].push(row);
        }

        for (let index in raidSquadRows) {
            let row = raidSquadRows[index];
            row.restrictions = groupedRestrictionRows[row.id];
        }

        let fields = [];

        for (let index in raidSquadRows) {
            let detail = raidSquadRows[index];
            let value = "Fill";
            if (detail.restrictions) {
                value = detail.restrictions.map(
                    restriction =>
                    {
                        let value = "";
                        if (restriction.profession)
                        {
                            value += `${restriction.profession_icon}`;
                            if (restriction.role) {
                                value += `${restriction.role || ""}`;
                            }
                        }
                        return value;
                    }
                ).join(" ");
            }

            fields.push({
                name: `Slot ${detail.spot}:`,
                value: `${value}: ${detail.user_id}`
            });
        }

        channel.send({embed: {
                color: 3447003,
                author: {
                    name: this.client.user.username,
                    icon_url: this.client.user.avatarURL
                },
                title: "Clear Summary",
                description: raidDescription,
                fields: clearFields,
                timestamp: new Date(),
                footer: {
                    icon_url: this.client.user.avatarURL,
                    text: "Wings and Bosses"
                }
            }
        });

        channel.send({embed: {
                color: 3447003,
                author: {
                    name: this.client.user.username,
                    icon_url: this.client.user.avatarURL
                },
                title: "Squad Slots Summary",
                description: "in order to join, please use the !enlist bot command",
                fields: fields,
                timestamp: new Date(),
                footer: {
                    icon_url: this.client.user.avatarURL,
                    text: "Slots"
                }
            }
        });
       
    }


    async run() {
        // Check permissions
        if (!this.hasPermission()) {
            this.message.reply("Insufficient permissions for action");
            return;
        }

        try {
            // Check if raid setup in progress
            let raid = await this.getRaid(this.message.member.id);
            this.message.reply(`Raid ID: ${raid.id}`);

            await this.setupRaid(raid);
        } catch (e) {
            console.error(e);
        }

    }
}

exports.run = (client, message, args) => {
    let raidSetup = new RaidSetup(client, message, args);
    raidSetup.run().catch(console.error);
};

