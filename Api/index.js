const express = require("express");
const bodyParser = require('body-parser');
const { MongoClient } = require("mongodb");
const mapRoutes = require('./routes/map');
const voteRoutes = require('./routes/vote');
const allRoutes = require('./routes/all');

// Replace this with your actual MongoDB connection string from an environment variable
const mongodbConnString = process.env.MONGODB_CONN_STRING;

const app = express();
const port = 30674;

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// MongoDB connection setup
async function initializeDatabase() {
  const client = new MongoClient(mongodbConnString);
  await client.connect();
  return client.db('CommunityLevelsDB').collection('CommunityLevelsColl');
}

app.use('/map', mapRoutes);
app.use('/vote', voteRoutes);
app.use('/all', allRoutes);

// Error handling middleware
app.use((err, req, res, next) => {
  console.error("Internal Server Error:", err);
  res.status(500).send("Internal Server Error");
});

app.listen(port, () => {
  console.log(`Server listening on port ${port}`);
});
