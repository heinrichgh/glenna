exports.run = async (client, messageReaction, user) => {
	if (user.bot) return;
	if (messageReaction.message.channel.id == 448592213203615744 || messageReaction.message.channel.id == 448943451707408414)
		messageReaction.message.channel.send(`User <@${user.id}> withdrew as ${messageReaction.emoji}`).catch(console.error);
}