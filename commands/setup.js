const moment = require("moment");
const sql = require("../util/sql");
const gwAPI = require("../gw2/api");
const config = require("../config.js");

class GuildSetup {

    constructor(client, message, args) {
        this.client = client;
        this.message = message;
        this.args = args;
    }

    hasPermission() {
        const owner = this.message.guild.owner;
        if (owner.user.id !== this.message.author.id) {
            console.log("Only the owner of this server can execute this command!");
            return false;
        }
        return true;
    }

    async getGuild(account, guild) {
        await this.createGuild(account, guild);
        let [rows] = await sql.execute('SELECT id FROM guild WHERE guild_api_id = ?', [guild.id]);
        if (rows.length > 0) {
            let sql_count_ranks = await sql.execute('SELECT COUNT(`id`) as c FROM `guild_rank` WHERE `guild_id` = (SELECT id FROM guild WHERE guild_api_id = ?)',[guild.id]);
            let sql_count_members = await sql.execute('SELECT COUNT(`id`) as c FROM `guild_member` WHERE `guild_id` = (SELECT id FROM guild WHERE guild_api_id = ?)',[guild.id]);
            return `Created guild: ${guild.name} [${guild.tag}]\nRanks: ${sql_count_ranks[0][0].c}\nMembers: ${sql_count_members[0][0].c}`;
        }
        
        return 'failed..';
    }

    // async removeGuild(guild) {
    //     let [rows] = await sql.execute('SELECT id FROM guild WHERE guild_api_id = ?', [guild.id]);
    //     for (var i = rows.length - 1; i >= 0; i--) {
    //         console.log(`Removing guild with ID: ${rows[i].id}`);
    //         await sql.execute('DELETE FROM guild WHERE id = ?', [rows[i].id]);
    //         await sql.execute('DELETE FROM guild_rank WHERE guild_id = ?', [rows[i].id]);
    //         await sql.execute('DELETE FROM guild_member WHERE guild_id = ?', [rows[i].id]);
    //     }
    // }

    async createRanks(guild)
    {
        let [rows] = await sql.execute('SELECT rank, is_starting FROM `guild_rank` WHERE guild_rank.guild_id = (SELECT id FROM guild WHERE guild.guild_api_id = ?) ORDER BY `rank_order` ASC',[guild.id]);
        for (var i = rows.length - 1; i >= 0; i--) {
            let role = this.message.guild.roles.find("name", rows[i].rank);
            if (!role) {
                this.message.guild.createRole({
                    name: rows[i].rank
                })
                .then(role => console.log(`Created new role with name ${role.name}.`))
                .catch(console.error)
            }
        }
    }

    async createGuild(account, guild) {
        // insert guild

        let [sql_result_guild] = await sql.execute('SELECT id FROM `guild` WHERE `guild_api_id` = ?',[guild.id]);
        if (sql_result_guild[0]) {
            console.log(sql_result_guild);
            await sql.execute('UPDATE `guild` SET guild_api_id = ?, name = ?, tag = ?, leader = ? WHERE id = ?',[guild.id, guild.name, guild.tag, account.id, sql_result_guild[0].id]);
        }
        else
            [sql_result_guild] = await sql.execute('INSERT INTO guild (id, guild_api_id, name, tag, leader) VALUES (?, ?, ?, ?, ?)',[null, guild.id, guild.name, guild.tag, account.id]);
        

        // insert ranks
        let gw2_api = new gwAPI(this.args);
        let ranks = await gw2_api.guild_ranks_lookup(guild.id);
        for (var i = ranks.length - 1; i >= 0; i--) {
            let startingRole = 0;
            if (ranks[i].permissions[0] == "StartingRole") { startingRole = 1}
            
            let [sql_guild_ranks] = await sql.execute('SELECT id FROM `guild_rank` WHERE guild_id IN (SELECT id FROM guild WHERE guild.guild_api_id = ?) AND guild_rank.rank LIKE ?',[guild.id, ranks[i].id]);
            console.log(sql_guild_ranks);
            if (sql_guild_ranks)
                await sql.execute('UPDATE guild_rank SET rank = ?, rank_order = ?, is_starting = ?, guild_id = ? WHERE id = ?',[ranks[i].id, ranks[i].order, startingRole, sql_result_guild[0].id, sql_guild_ranks[0].id]);   
            else
                await sql.execute('INSERT INTO guild_rank (id, rank, rank_order, is_starting, guild_id) VALUES (?, ?, ?, ?, ?)',[null, ranks[i].id, ranks[i].order, startingRole, sql_result_guild[0].id]);
        }
        
        // insert members
        let members = await gw2_api.guild_members_lookup(guild.id);
        for (var i = members.length - 1; i >= 0; i--) {
            if (members[i].rank.trim() != "invited")
            {
                let [rows] = await sql.execute('SELECT * FROM `guild_rank` WHERE rank LIKE ?',[members[i].rank]);
                let [sql_member_id] = await sql.execute('SELECT id FROM `guild_member` WHERE guild_member_name LIKE ? AND guild_id = ?',[members[i].name, sql_result_guild[0].id]);
                console.log(sql_member_id);
                if  (sql_member_id[0].id)
                    await sql.execute('UPDATE `guild_member` SET `guild_id` = ?, `guild_member_name` = ?, `rank_id` = ? WHERE id = ? AND guild_member_name = ?',[sql_result_guild[0].id, members[i].name, rows[0].id, sql_member_id[0].id, members[i].name]);
                else
                    await sql.execute('INSERT INTO `guild_member` (`id`, `guild_id`, `guild_member_name`, `discord_id`, `rank_id`, `api_key`) VALUES (?, ?, ?, ?, ?, ?)',[null, sql_result_guild[0].id, members[i].name, "", rows[0].id, null]);
            }
        }
    }

    async run() {
        if (!this.hasPermission()) {
            this.message.reply("Insufficient permissions for action");
            return;
        }

        try {

            let gw2_api = new gwAPI(this.args);
            let account = await gw2_api.account_lookup();
            var msg = "";
            var guilds = [];

            if (account.guild_leader.length)
            {
                for (var i = account.guild_leader.length - 1; i >= 0; i--) {
                    var guild_info = await gw2_api.guild_lookup(account.guild_leader[i]);
                    msg += `${i+1} - ${guild_info.name} [${guild_info.tag}]\n`;
                    guilds.push(guild_info);
                }
                msg += "Which guild you would like to add?";

                if (guilds.length > 0) {
                
                    this.message.reply(msg)
                    const filter = m => m.member === this.message.member;
                    let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 30000});
                
                    if (!collected.size) {
                        this.message.reply(`please respond with 1-${guilds.length}`);
                        return;
                    }
                
                    let num = collected.first().content.trim();
                    if (num > 0 && num <=(guilds.length) ) {
                        // await this.removeGuild(guilds[num-1]);
                        this.message.reply(await this.getGuild(account, guilds[num-1]));
                        await this.createRanks(guilds[num-1]);
                    }
                    else { this.message.reply(`please respond with 1-${guilds.length}`); }
                }
                else { this.message.reply("You need lead at least one guild to use this command."); }
            }
        } catch (e) {
            console.error(e);
        }
    }
}

exports.run = (client, message, args) => {
    var pattern = /[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*/g;
    if (pattern.test(args)) {
        message.delete();
        let guildSetup = new GuildSetup(client, message, args);
        guildSetup.run().catch(console.error);    
    }
    else
        message.reply("Usage:```!setup [api-key]```");
};




