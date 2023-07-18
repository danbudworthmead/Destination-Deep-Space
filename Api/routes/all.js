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
      delete map.date;
      delete map.completions;
      map.id = map._id.toString();
      delete map._id;
      maps.push(map);
    }

    // Sort the maps by score in descending order
    maps.sort((mapA, mapB) => mapB.score - mapA.score);
    
    // Take only the top 20 rated maps
    const topRatedMaps = maps.slice(0, 20);

    res.status(200).send({maps: topRatedMaps});
  } catch (error) {
    console.error("Error retrieving maps:", error);
    res.status(500).send("Internal Server Error");
  }
});

module.exports = router;
