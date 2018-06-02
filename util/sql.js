const mysql = require('mysql2/promise');
const config = require("../config.js");

module.exports = {
    execute: async function(query, params) {
        const connection = await mysql.createConnection(config.db);
        if (params) {
            return await connection.execute(query, params);
        } else {
            return await connection.execute(query);
        }
    },

    query: async function(query) {
        const connection = await mysql.createConnection(config.db);
            return await connection.query(query);
    },
};