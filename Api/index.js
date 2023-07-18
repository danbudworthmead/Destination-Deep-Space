const { MongoClient } = require("mongodb");
const express = require("express");
const bodyParser = require('body-parser');

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

async function main() {
  const app = express();
  const port = 30674;

  app.use(bodyParser.json());
  app.use(bodyParser.urlencoded({ extended: true }));

  app.post("/map", async (req, res) => {
    console.log(`${req.hostname} used POST/map`);
    const levelData = req.body;

    // do some checking here on levelData

    try {
      const collection = await initializeDatabase();
      await addMapToCollection(levelData, collection);
      res.status(200).send();
    } catch (error) {
      console.error("Error adding map:", error);
      res.status(500).send("Internal Server Error");
    }
  });

  app.get("/map/:id", async (req, res) => {
    const { id } = req.params;
    console.log(`${req.hostname} used GET/map/${id}`);

    try {
      const collection = await initializeDatabase();
      const mapData = await collection.findOne({ id: parseInt(id) });
      if (mapData) {
        res.status(200).send(mapData);
      } else {
        res.status(404).send(`Map with id ${id} not found`);
      }
    } catch (error) {
      console.error("Error retrieving map:", error);
      res.status(500).send("Internal Server Error");
    }
  });

  app.get("/all", async (req, res) => {
    try {
      const collection = await initializeDatabase();
      const cursor = collection.find();
      const maps = [];
      for await (const map of cursor) {
        delete map.props;
        map.score = map.votes.up - map.votes.down;
        delete map.votes;
        maps.push(map);
      }

      // Sort the maps by score in descending order
      maps.sort((mapA, mapB) => mapB.score - mapA.score);

      console.log(maps);
      res.status(200).send(maps);
    } catch (error) {
      console.error("Error retrieving maps:", error);
      res.status(500).send("Internal Server Error");
    }
  });

  app.post("/vote/:id/:score", async (req, res) => {
    const id = parseInt(req.params.id);
    const score = parseInt(req.params.score);

    if (score !== -1 && score !== 1) {
      res.status(400).send(`Invalid vote of ${score}`);
      return;
    }

    try {
      const collection = await initializeDatabase();
      const map = await collection.findOne({ id: id });

      if (!map) {
        res.status(404).send(`Map with id ${id} not found`);
        return;
      }

      if (score === 1) {
        map.votes.up++;
      } else if (score === -1) {
        map.votes.down++;
      }

      const options = { upsert: false };

      // update the doc
      await collection.updateOne({ id: id }, {
        "$set": {
          votes: map.votes,
          completions: ++map.completions,
        }
      }, options);

      // send the response
      res.status(200).send(`Score of ${id} is now ${map.votes.up - map.votes.down}`);
    } catch (error) {
      console.error("Error voting on map:", error);
      res.status(500).send("Internal Server Error");
    }
  });

  // Error handling middleware
  app.use((err, req, res, next) => {
    console.error("Internal Server Error:", err);
    res.status(500).send("Internal Server Error");
  });

  app.listen(port, () => {
    console.log(`Server listening on port ${port}`);
  });
}

main().catch((error) => {
  console.error("Error starting the server:", error);
});
