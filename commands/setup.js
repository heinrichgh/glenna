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

    async getGuild(account, guild) {
        let [rows] = await sql.execute(`SELECT name FROM guild WHERE guild_api_id = ?`, guild.id);
        
        if (rows.length = 1) {
            return `${guild.name} [${guild.tag}]\n
            Ranks: (todo)\n
            Members: (todo)`;
        }
        this.createGuild(guild);
        return getGuild(guild);
    }

    async createGuild(account, guild) {
        await sql.execute(`DELETE FROM guild WHERE guild_api_id = ?`, guild.id);
        var result = sql.execute(`INSERT INTO guild (id, guild_api_id, name, tag, leader) VALUES (NULL, '${guild.id}', '${guild.name}', '${guild.tag}', '${account.id}')`);
        // insert ranks
        // insett members
    }

    async run() {
        if (!this.hasPermission()) {
            this.message.reply("Insufficient permissions for action");
            return;
        }

        try {

            let gw2_api = new gwAPI(this.args);
            let account = gw2_api.account_lookup();
            console.log(account)
            // var msg = "";
            //
            // for (var i = account.guild_leader.length - 1; i >= 0; i--) {
            //     var guild_info = gw2_api.guild_lookup(account.guild_leader[i]);
            //     msg += `${i} - ${guild_info.name} [${guild_info.tag}]\n`;
            // }
            //
            // if (guilds.length > 0) {
            //
            //     this.message.reply(msg)
            //     const filter = m => m.member === this.message.member;
            //     let collected = await this.message.channel.awaitMessages(filter, {max: 1, time: 30000});
            //
            //     if (!collected.size) {
            //         this.message.reply(`please respond with 1-${guilds.length}`);
            //         return;
            //     }
            //
            //     let num = collected.first().content.trim();
            //     if (num > 0 && num <=(guilds.length) ) {
            //         this.message.reply(getGuild(account, guilds[num]));
            //     }
            //     else { this.message.reply(`please respond with 1-${guilds.length}`); }
            // }
            // else { this.message.reply("not a leader"); }
        } catch (e) {
            console.error(e);
        }

    }
}

exports.run = (client, message, args) => {
    // var pattern = /[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*\-[A-Z0-9]*/g;
    // message.reply("Not Implemented! Demo Only")
    let guildSetup = new GuildSetup(client, message, args);
    guildSetup.run().catch(console.error);
};




