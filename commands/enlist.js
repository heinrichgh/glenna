const moment = require("moment");
const sql = require("../util/sql");
const config = require("../config.js");
const reaction_numbers = ["\u0030\u20E3","\u0031\u20E3","\u0032\u20E3","\u0033\u20E3","\u0034\u20E3","\u0035\u20E3", "\u0036\u20E3","\u0037\u20E3","\u0038\u20E3","\u0039\u20E3"];
const STATUS = {
    AWAITING_DATE: 1,
    AWAITING_CLEAR_TYPE: 2,
    AWAITING_TEMPLATE: 3,
    AWAITING_PUBLISH: 4,
    PUBLISHED: 5,
    FULL: 6,
    COMPLETED: 10
};

class Enlist {

    constructor(client, message, args) {
        this.client = client;
        this.message = message;
        this.args = args;
    }

    hasRole() {
        // Not implemented yet
        return true;
    }

    async setRole(raidId, role) {
        let [raidSquadId] = await sql.execute(`SELECT raid_squad.id
            FROM raid_squad 
            JOIN raid_squad_restriction ON raid_squad.id = raid_squad_restriction.raid_squad_id
            JOIN raid_role ON raid_squad_restriction.raid_role_id = raid_role.id
            JOIN guild_rank on raid_squad_restriction.guild_rank_id = guild_rank.id
            WHERE raid_squad.raid_id = ${raidId}
            AND guild_rank.rank_order >= (SELECT guild_rank.rank_order FROM guild_rank JOIN guild_member ON guild_member.rank_id = guild_rank.id WHERE guild_member.discord_id = ${this.message.member.id})
            AND raid_squad.user_id IS NULL
            AND raid_role.title LIKE '${role}'
            LIMIT 1`);
        await sql.execute(`UPDATE raid_squad SET user_id = NULL WHERE raid_id = ${raidId} AND user_id = '<@${this.message.member.id}>'`);
        await sql.execute(`UPDATE raid_squad SET user_id = '<@${this.message.member.id}>' WHERE raid_squad.id = ${raidSquadId[0].id}`);
        console.log(`Set ${this.message.member.id} role to ${role} for raid ${raidId}`);
        let chan = this.message.guild.channels.find('id', config.raidChannelId);
        await this.updateSchedule(raidId, chan);
    }

    async sendSummary(raid, channel) {
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
              rcs.raid_id = ${raid.id}
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
                `SELECT *
                FROM raid_squad
                JOIN raid_squad_restriction ON raid_squad.id = raid_squad_restriction.raid_squad_id
                JOIN guild_rank on raid_squad_restriction.guild_rank_id = guild_rank.id
                WHERE raid_squad.raid_id = ${raid.id}
                AND user_id IS NULL
                AND guild_rank.rank_order >= (SELECT guild_rank.rank_order FROM guild_rank JOIN guild_member ON guild_member.rank_id = guild_rank.id WHERE guild_member.discord_id = ${this.message.member.id})
                ORDER BY spot`);

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
          JOIN guild_rank on guild_rank.id = rsr.guild_rank_id
        WHERE squad.raid_id = ${raid.id}
        AND squad.user_id IS NULL
        AND guild_rank.rank_order >= (SELECT guild_rank.rank_order FROM guild_rank JOIN guild_member ON guild_member.rank_id = guild_rank.id WHERE guild_member.discord_id = ${this.message.member.id})
        ORDER BY
            squad.spot`);

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
            raidSquadRows[index].restrictions = groupedRestrictionRows[row.raid_squad_id];
        }
        let fields = [];
        let reactions = [];

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
                            reactions.push(`${restriction.profession_icon}`);
                            value += `${restriction.profession_icon}`;
                            if (restriction.role) {
                                value += `${restriction.role_icon || ""}`;
                            }
                        }
                        return value;
                    }
                ).join(" ");
            }

            fields.push({
                name: `Slot ${detail.spot}:`,
                value: value
            });
        }

        channel.send({embed: {
                color: 3447003,
                author: {
                    name: this.client.user.username,
                    icon_url: this.client.user.avatarURL
                },
                title: "Clear Summary",
                fields: clearFields,
                timestamp: new Date(),
                footer: {
                    icon_url: this.client.user.avatarURL,
                    text: "Wings and Bosses"
                }
            }
        })
        .then((message) => {
            if (reactions.length >0) {
                for (var i = reactions.length - 1; i >= 0; i--) {
                    var regex = /<:[a-z]*:([0-9]*)>/g;
                    var emojiarray = regex.exec(reactions[i]);
                    let emoji = message.guild.emojis.get(emojiarray[1]);
                    (async function (){
                        await message.react(emoji);
                    })()
                }

                // wait for response
                const filter = (reaction, user) => user.id === this.message.member.id;
                const collector = message.createReactionCollector(filter, { max: 1 });
                let response = "Please select a role:\n";
                let count = 0;
                let role = [];
                const discord_id = this.message.member.id;
                collector.on('collect', (r) => {
                    (async function (){ 
                        let [roles] = await sql.execute(`
                            SELECT DISTINCT(raid_role.title) 
                            FROM raid_role 
                            JOIN raid_squad_restriction ON raid_role.id = raid_squad_restriction.raid_role_id 
                            JOIN raid_squad ON raid_squad.id = raid_squad_restriction.raid_squad_id 
                            JOIN profession ON raid_squad_restriction.profession_id = profession.id 
                            JOIN guild_rank on raid_squad_restriction.guild_rank_id = guild_rank.id 
                            WHERE raid_squad.raid_id = ${raid.id}
                            AND guild_rank.rank_order >= (SELECT guild_rank.rank_order FROM guild_rank JOIN guild_member ON guild_member.rank_id = guild_rank.id WHERE guild_member.discord_id = ${discord_id}) 
                            AND raid_squad.user_id IS NULL
                            AND profession.title LIKE '${r.emoji.name}'`);
                        
                        count = roles.length;
                        for (var i = 0; roles.length-1 >= i; i++) {
                            response += `${i+1} - ${roles[i].title}\n`;
                            role.push(roles[i].title);
                        }
                    })()
                });
                collector.on('end', collected => {
                    (async function (){
                        await new Promise(resolve => setTimeout(resolve, 500));
                    })()
                    this.message.reply(response)
                    .then((msg) => {
                            for (var i = 1; count >= i; i++) {
                                (async function (){
                                    await msg.react(reaction_numbers[i]);
                                })()
                            }
                            const filter = (reaction, user) => user.id === this.message.member.id;
                            const collector = msg.createReactionCollector(filter, { max: 1 });
                            collector.on('collect', (rr) => {
                                switch (rr.emoji.name)
                                {
                                    case '1⃣':
                                        this.setRole(raid.id, role[0]);
                                        break;

                                    case '2⃣':
                                        this.setRole(raid.id, role[1]);
                                        break;

                                    case '3⃣':
                                        this.setRole(raid.id, role[2]);
                                        break;

                                    case '4⃣':
                                        this.setRole(raid.id, role[3]);
                                        break;

                                    case '5⃣':
                                        this.setRole(raid.id, role[4]);
                                        break;

                                    case '6️⃣':
                                        this.setRole(raid.id, role[5]);
                                        break;

                                    case '7️⃣':
                                        this.setRole(raid.id, role[6]);
                                        break;

                                    case '8️⃣':
                                        this.setRole(raid.id, role[7]);
                                        break;

                                    case '9️⃣':
                                        this.setRole(raid.id, role[8]);
                                        break;

                                    case '🔟':
                                        this.setRole(raid.id, role[9]);
                                        break;
                                }
                            });
                            collector.on('end', collected => msg.delete());
                        }).catch(function() {});
                    message.delete();
                });
            }
            else
            {
                message.reply("No spots available for your rank. Have you set up your rank with !join?");
            }
        }).catch(function() {});
    }

    async updateSchedule(raid, channel) {
        let fetched = await channel.fetchMessages({limit: 99});
        channel.bulkDelete(fetched);

        fetched = await channel.fetchMessages({limit: 99});
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

        let raidDescription = "";

        for (let index in ranks)
            raidDescription += `${this.message.guild.roles.find("name", ranks[index].rank)} `;
        raidDescription += `${this.message.guild.roles.find("name", "Friends of FluX")} `;

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
                timestamp: new Date(), // change to raid time
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
                timestamp: new Date(), // change to raid time
                footer: {
                    icon_url: this.client.user.avatarURL,
                    text: "Slots"
                }
            }
        });
       
    }

    async run() {
        if (!this.hasRole()) {
            this.message.reply("Insufficient permissions for action");
            return;
        }

        try {
            let [raid] = await sql.execute('SELECT * FROM `raid` WHERE status = ? LIMIT 1',[STATUS.PUBLISHED]);
            if (raid.length) {
                await this.sendSummary(raid[0], this.message.channel);    
            }
        } catch (e) {
            console.error(e);
        }
    }
}

exports.run = (client, message, args) => {
    let enlistment = new Enlist(client, message, args);
    enlistment.run().catch(console.error);
};




