exports.run = (client, member) => {
	const defaultChannel = member.guild.channels.find('id', 95237486174810112);
	member.message.send(`Welcome ${member.user} to the State Of FluX Discord.`).catch(console.error);
}