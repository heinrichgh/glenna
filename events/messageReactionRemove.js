const config = require("../config.js");

exports.run = async (client, messageReaction, user) => {
	if (user.bot) return;

	// Handle raid channel
	if (messageReaction.message.channel.id == config.raidChannelId)
		messageReaction.message.channel.send(`User <@${user.id}> withdrew as ${messageReaction.emoji}`).catch(console.error);
}