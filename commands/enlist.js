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

class Enlist {

    constructor(client, message, args) {
        this.client = client;
        this.message = message;
        this.args = args;
    }

    hasPermission() {
        const leaderRole = this.message.guild.roles.find("name", "Leaders");
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

   async sendSummary(raid, channel) {
        let [clearTypeRows] = await
            sql.execute(`
            SELECT
              rw.name as 'WingName',
              rb.name as 'BossName'
            FROM
              raid_clear_setup rcs
              INNER JOIN raid_boss rb on rcs.raid_boss_id = rb.id
              INNER JOIN raid_wing rw on rb.raid_wing_id = rw.id
            WHERE
              rcs.raid_id = ?
            ORDER BY
              rw.id,
              rb.number`, [raid.id]);


        let groupedClearTypes = {};
        for (let clearType of clearTypeRows) {
            if (!groupedClearTypes[clearType.WingName]) {
                groupedClearTypes[clearType.WingName] = {wing: clearType.WingName, data: []}
            }
            groupedClearTypes[clearType.WingName].data.push(clearType.BossName);

        }

        let clearFields = [];
        for (let index in groupedClearTypes) {
            let groupedClearType = groupedClearTypes[index];
            clearFields.push({
                name: `${groupedClearType.wing}:`,
                value: groupedClearType.data.join("\n")
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
        });

        let [raidSquadRows] = await
            sql.execute(
                `SELECT
          *
        FROM
          raid_squad
        WHERE
          raid_id = ?
        ORDER BY
           spot`, [raid.id]);

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
          squad.raid_id = ${raid.id}
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
            row.restrictions = groupedRestrictionRows[row.id];
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
                            value += `${restriction.profession} ${restriction.profession_icon}`;
                            if (restriction.role) {
                                value += ` as ${restriction.role} ${restriction.role_icon || ""}`;
                            }
                        } else {
                            if (restriction.role) {
                                value += `${restriction.role} ${restriction.role_icon || ""}`;
                            }
                        }
                        return value;
                    }
                ).join("\n");
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
                title: "Squad Slots Summary",
                fields: fields,
                timestamp: new Date(),
                footer: {
                    icon_url: this.client.user.avatarURL,
                    text: "Slots"
                }
            }
        })
        .then(function (message) {
            if (reactions.length >0) {
                for (var i = reactions.length - 1; i >= 0; i--) {
                    var regex = /<:[a-z]*:([0-9]*)>/g;
                    var emojiarray = regex.exec(reactions[i]);
                    let emoji = message.guild.emojis.get(emojiarray[1]);
                    message.react(emoji);
                }
            }
        }).catch(function() {
              //Something
             });
    }



    async run() {
        if (!this.hasPermission()) {
            this.message.reply("Insufficient permissions for action");
            return;
        }

        try {
            let [raid] = await sql.execute('SELECT * FROM `raid` WHERE status = ? LIMIT 1',[STATUS.PUBLISHED]);
            console.log(raid[0].id);
            if (raid.length) {
                //await this.sendSummary(raid[0], this.message.channel);    
            }
            const emojiList = message.guild.emojis.map(e=>e.toString()).join(" ");
            console.log(emojiList);
            this.message.channel.send(emojiList);


        } catch (e) {
            console.error(e);
        }
    }
}

exports.run = (client, message, args) => {
    message.reply("Not Implemented! Demo Only");
    let enlistment = new Enlist(client, message, args);
    enlistment.run().catch(console.error);
};




