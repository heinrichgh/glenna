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
            this.message.delete();
            let gw2_api = new gwAPI(this.args);
            let account = await gw2_api.account_lookup();
            
            let [guild] = await sql.execute('SELECT guild_member.guild_member_name, guild_rank.rank FROM `guild_member` JOIN guild ON guild_member.guild_id = guild.id JOIN guild_rank ON guild_member.rank_id = guild_rank.id WHERE guild_member.guild_member_name = ? ORDER BY `rank_id` ASC',[account.name]);
            if (guild[0]) {
                const role = this.message.guild.roles.find("name", guild[0].rank)
                await sql.execute('UPDATE `guild_member` SET `discord_id` = ?, `api_key` = ? WHERE `guild_member`.`guild_member_name` = ?',[this.message.author.id, this.args[0], guild[0].guild_member_name]);
                this.message.member.addRole(role);
                this.message.member.setNickname(guild[0].guild_member_name)
                    .then(console.log)
                    .catch(console.error);
                this.message.reply(`Hi ${guild[0].guild_member_name} your rank has been set to '${guild[0].rank}'`);
            }
            else {
                this.message.reply("User not found in <guildnamehere>, would you like to join as <friend>? (yes/no)");
                const filter = m => m.member === this.message.member;
                let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 30000});

                if (!collected.size) {
                    this.message.reply(`No response received`);
                    return;
                }
                if (collected.first().content.trim() == "yes")
                {
                    let friendRole = this.message.guild.roles.find("name", "Friend Of FluX");

                    if (!friendRole) {
                        this.message.guild.createRole({
                            name: "Friend Of FluX"
                        })
                        .then(role => console.log(`Created new role with name ${role.name}.`))
                        .catch(console.error)    
                    }
                    let [sql_rank_id] = await sql.execute('SELECT id, guild_id FROM `guild_rank` WHERE is_starting = 1');
                    if (sql_rank_id[0]) {
                        let [sql_member_id] = await sql.execute('SELECT id FROM `guild_member` WHERE guild_member_name LIKE ? AND guild_id = ?',[account.name, sql_rank_id[0].guild_id]);
                        if (sql_member_id[0])
                            await sql.execute('UPDATE `guild_member` SET `guild_id` = ?, `guild_member_name` = ?, `rank_id` = ? WHERE id = ? AND guild_member_name = ?',[sql_rank_id[0].guild_id, account.name, sql_rank_id[0].id, sql_member_id[0].id, account.name]);
                        else
                            await sql.execute('INSERT INTO `guild_member` (`id`, `guild_id`, `guild_member_name`, `discord_id`, `rank_id`, `api_key`) VALUES (?, ?, ?, ?, ?, ?)',[null, sql_rank_id[0].guild_id, account.name, "", sql_rank_id[0].id, this.args]);
                    }
                    friendRole = this.message.guild.roles.find("name", "Friend Of FluX");
                    this.message.member.addRole(friendRole);
                    this.message.member.setNickname(account.name)
                        .then(console.log)
                        .catch(console.error);
                    this.message.reply(`Hi ${account.name} your rank has been set to '${friendRole}'`);
                }
            }
        } catch (e) {
            console.error(e);
        }

    }
}

exports.run = (client, message, args) => {
    var pattern = /[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*/g;
    if (pattern.test(args)) {
        let accountSetup = new AccountSetup(client, message, args);
        accountSetup.run().catch(console.error);    
    }
    else
        message.reply("Usage:```!join [api-key]```");
};




