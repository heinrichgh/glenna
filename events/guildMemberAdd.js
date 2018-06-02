exports.run = (client, member) => {
	const defaultChannel = member.guild.channels.find(c=> c.permissionsFor(guild.me).has("SEND_MESSAGES"));
	member.message.send(`Welcome ${member.user} to the State Of FluX Discord.`).catch(console.error);
}