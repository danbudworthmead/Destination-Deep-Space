const express = require("express");
const router = express.Router();
const { initializeDatabase } = require('../helpers/dbHelpers');

router.get("/", async (req, res) => {
  try {
    const collection = await initializeDatabase();
    const cursor = collection.find();
    const currentDate = new Date();
    const topRatedMaps = [];

    for await (const map of cursor) {
      const mapDate = new Date(map.date);
      const diffTime = Math.abs(currentDate - mapDate);
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
      
      if (diffDays <= 7) {
        map.score = map.votes.up - map.votes.down;
        delete map.props;
        delete map.votes;
        delete map.completions;
        delete map._id;
        delete map.date;
        topRatedMaps.push(map);
      }
    }

    // Sort the top-rated maps by score in descending order
    topRatedMaps.sort((mapA, mapB) => mapB.score - mapA.score);

    // Take only the top 20 rated maps
    const top20Maps = topRatedMaps.slice(0, 20);

    res.status(200).send({ maps: top20Maps });
  } catch (error) {
    console.error("Error retrieving maps:", error);
    res.status(500).send("Internal Server Error");
  }
});

module.exports = router;
