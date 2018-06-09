const mysql = require('mysql2/promise');
const config = require("../config.js");

module.exports = {
    execute: async function(query, params) {
        const connection = await mysql.createConnection(config.db);
        if (params) {
            let result = await connection.execute(query, params);
            await connection.end();
            return result;

        } else {
            let result = await connection.execute(query);
            await connection.end();
            return result;        
        }
    },

    query: async function(query) {
        const connection = await mysql.createConnection(config.db);
            let result = await connection.query(query);
            await connection.end();
            return result; 
    },
};