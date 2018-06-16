const moment = require("moment");
const sql = require("../util/sql");
const config = require("../config.js");

exports.run = (client, message, args) => {
    message.reply("Helpfile incoming```!help: this crap\n!join [api-key]: Sets your discord permissions per guild rank\n!enlist: TODO - Join a raid squad\n!raid: Schedules a raid\n!setup [api-key]: (Discord Owner only) Creates guild ranks and populates member list\n!reload [command]: Updates command code without restarting bot```");
    
};