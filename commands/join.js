const moment = require("moment");
const sql = require("../util/sql");
const gwAPI = require("../gw2/api");
const config = require("../config.js");

class AccountSetup {

    constructor(client, message, args) {
        this.client = client;
        this.message = message;
        this.args = args;
    }

    async run() {
        try {

            let gw2_api = new gwAPI(this.args);
            let account = await gw2_api.account_lookup();
            
            let guild = await sql.execute('SELECT guild_member.guild_member_name, guild_rank.rank FROM `guild_member` JOIN guild ON guild_member.guild_id = guild.id JOIN guild_rank ON guild_member.rank_id = guild_rank.id WHERE guild_member.guild_member_name = ? ORDER BY `rank_id` ASC',[account.name]);
            
            const role = this.message.guild.roles.find("name", guild[0][0].rank);
            if (role) {
                await sql.execute('UPDATE `guild_member` SET `discord_id` = ? WHERE `guild_member`.`guild_member_name` = ?',[this.message.author.id, guild[0][0].guild_member_name]);
                this.message.member.addRole(role);
                this.message.member.setNickname(guild[0][0].guild_member_name)
                    .then(console.log)
                    .catch(console.error);
                this.message.reply(`Hi ${guild[0][0].guild_member_name} your rank has been set to '${guild[0][0].rank}'`);
            }
            else {
                console.log("role does not exist, perhaps rerun !setup?")
            }
            // if (account.guild_leader.length)
            // {
            //     for (var i = account.guild_leader.length - 1; i >= 0; i--) {
            //         var guild_info = await gw2_api.guild_lookup(account.guild_leader[i]);
            //         msg += `${i+1} - ${guild_info.name} [${guild_info.tag}]\n`;
            //         guilds.push(guild_info);
            //     }
            //     msg += "Which guild you would like to add?";

            //     if (guilds.length > 0) {
                
            //         this.message.reply(msg)
            //         const filter = m => m.member === this.message.member;
            //         let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 30000});
                
            //         if (!collected.size) {
            //             this.message.reply(`please respond with 1-${guilds.length}`);
            //             return;
            //         }
                
            //         let num = collected.first().content.trim();
            //         if (num > 0 && num <=(guilds.length) ) {
            //             await this.removeGuild(guilds[num-1]);
            //             this.message.reply(await this.getGuild(account, guilds[num-1]));
            //             await this.createRanks(guilds[num-1]);
            //         }
            //         else { this.message.reply(`please respond with 1-${guilds.length}`); }
            //     }
            //     else { this.message.reply("not a leader"); }
            // }
        } catch (e) {
            console.error(e);
        }

    }
}

exports.run = (client, message, args) => {
    message.reply("Not Implemented! Demo Only");
    var pattern = /[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*/g;
    if (pattern.test(args)) {
        let accountSetup = new AccountSetup(client, message, args);
        accountSetup.run().catch(console.error);    
    }
    
};




