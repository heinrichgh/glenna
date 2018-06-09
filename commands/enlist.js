const moment = require("moment");
const sql = require("../util/sql");
const config = require("../config.js");

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

  

    async run() {
        if (!this.hasPermission()) {
            this.message.reply("Insufficient permissions for action");
            return;
        }

        try {
                   
        } catch (e) {
            console.error(e);
        }

    }
}

exports.run = (client, message, args) => {
    message.reply("Not Implemented! Demo Only")
    let enlistment = new Enlist(client, message, args);
    enlistment.run().catch(console.error);
};




