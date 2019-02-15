using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IGuildMemberRepository : IRepository<GuildMember>
    {
        GuildMember Load(int member, int guildId);
        GuildMember Save(GuildMember guildMember);
        GuildMember Delete(int id);
    }
}