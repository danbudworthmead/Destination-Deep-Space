const { MongoClient } = require("mongodb");

// Replace this with your actual MongoDB connection string from an environment variable
const mongodbConnString = process.env.MONGODB_CONN_STRING;

async function initializeDatabase() {
  const client = new MongoClient(mongodbConnString);
  await client.connect();
  return client.db('CommunityLevelsDB').collection('CommunityLevelsColl');
}

async function addMapToCollection(levelData, collection) {
  const level = {
    date: new Date().toJSON(),
    id: await collection.countDocuments(),
    name: levelData.name,
    props: levelData.props,
    votes: {
      up: 0,
      down: 0,
    },
    completions: 0,
  };
  await collection.insertOne(level);
  console.log(`Added map ${level.name}`);
}

module.exports = { initializeDatabase, addMapToCollection };
