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
        // const guildRole = this.message.guild.roles.map(m=>m.name);

        // console.log('printing roles..');
        // for (var i = guildRole.length - 1; i >= 0; i--) {
            
        //     console.log(guildRole[i]);
        // }

        return true;
    }

    async setRole(raidId, role) {
        //insert tole
        console.log(`Set ${this.message.member.id} role to ${role} for raid ${raidId}`);
        this.message.reply(`Set ${this.message.member.id} role to ${role} for raid ${raidId}`);
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
              rcs.raid_id = ?
            ORDER BY
              rw.id,
              rb.number`, [raid.id]);


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
                    message.react(emoji);
                }
            }
            // wait for response
            const filter = (reaction, user) => user.id === this.message.member.id;
            const collector = message.createReactionCollector(filter, { time: 10000 });
            let response = "Please select a role:\n";
            let count = 0;
            let role = [];
            collector.on('collect', (r) => {
                //const sql = require("../util/sql");

                (async function (){
                    let [roles] = await sql.execute(`
                        SELECT raid_role.title 
                        FROM raid_role 
                        JOIN raid_squad_restriction ON raid_role.id = raid_squad_restriction.raid_role_id 
                        JOIN raid_squad ON raid_squad.id = raid_squad_restriction.raid_squad_id 
                        JOIN profession ON raid_squad_restriction.profession_id = profession.id 
                        WHERE raid_squad.raid_id = ${raid.id} 
                        AND profession.title LIKE '${r.emoji.name}'`);
                    
                    count = roles.length;
                    for (var i = 0; roles.length-1 >= i; i++) {
                        response += `${i+1} - ${roles[i].title}\n`;
                        role.push(roles[i].title);
                    }
                })()

                // this.message.reply(`Enlisted as ${r.emoji.name} for raid: ${raid.id}`);
                message.delete();
            });
            collector.on('end', collected => {
                this.message.reply(response)
                .then((msg) => {
                        for (var i = 1; count >= i; i++) {
                            msg.react(reaction_numbers[i]);
                        }
                        const filter = (reaction, user) => user.id === this.message.member.id;
                        const collector = msg.createReactionCollector(filter, { time: 10000 });
                        collector.on('collect', (rr) => {
                            (async function (){
                                switch (rr.emoji.name)
                                {
                                    case '1⃣':
                                        console.log(role[0]);
                                        this.setRole(raid.id, role[0]);
                                        break;

                                    case '2⃣':
                                        console.log(role[1]);
                                        this.setRole(raid.id, role[1]);
                                        break;

                                    case '3⃣':
                                        console.log(role[2]);
                                        this.setRole(raid.id, role[2]);
                                        break;

                                    case '4⃣':
                                        console.log(role[3]);
                                        this.setRole(raid.id, role[3]);
                                        break;

                                    case '5⃣':
                                        console.log(role[4]);
                                        this.setRole(raid.id, role[4]);
                                        break;

                                    case '6️⃣':
                                        console.log(role[5]);
                                        this.setRole(raid.id, role[5]);
                                        break;

                                    case '7️⃣':
                                        console.log(role[6]);
                                        this.setRole(raid.id, role[6]);
                                        break;

                                    case '8️⃣':
                                        console.log(role[7]);
                                        this.setRole(raid.id, role[7]);
                                        break;

                                    case '9️⃣':
                                        console.log(role[8]);
                                        this.setRole(raid.id, role[8]);
                                        break;

                                    case '🔟':
                                        console.log(role[9]);
                                        this.setRole(raid.id, role[9]);
                                        break;
                                }
                            })()

                            //this.msg.reply(`Enlisted as ${rr.emoji.name} for raid: ${raid.id}`);
                            msg.delete();
                        });
                        collector.on('end', collected => msg.delete());
                    }).catch(function() {
                      //Something
                     });
                // message.delete();
            });
        }).catch(function() {
              //Something
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
    message.reply("Not Implemented! Demo Only");
    let enlistment = new Enlist(client, message, args);
    enlistment.run().catch(console.error);
};




