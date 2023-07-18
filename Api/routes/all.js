const express = require("express");
const router = express.Router();
const { initializeDatabase } = require('../helpers/dbHelpers');

router.get("/", async (req, res) => {
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
    res.status(200).send(maps);
  } catch (error) {
    console.error("Error retrieving maps:", error);
    res.status(500).send("Internal Server Error");
  }
});

module.exports = router;
