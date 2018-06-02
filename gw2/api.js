const axios = require('axios');

const apiUrl = 'https://api.guildwars2.com/v2';

async function api_query(url) {
    try {
        const response = await axios.get(url);
        return response.data;
    }
    catch (error) {
        console.error(error);
    }
}

class gw2_api {
    constructor(account_token) {
        this.account_token = account_token;
    }

    async account_lookup() {
        return await api_query(apiUrl + `/account?access_token=${this.account_token}`);
    }

    async guild_lookup(guild_leader) {
        return await api_query(apiUrl + `/guild/${guild_leader}?access_token=${this.account_token}`);
    }

    async guild_members_lookup(guild_id) {
        return await api_query(apiUrl + `/guild/${guild_id}/members?access_token=${this.account_token}`);
    }
}

exports = gw2_api;


