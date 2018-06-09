const config = require("../config.js");

exports.run = (client, member) => {
	const defaultChannel = member.guild.channels.find('id', config.defaultChannel);
	member.message.send(`Welcome ${member.user} to ${config.welcomeMsg}.`).catch(console.error);
}