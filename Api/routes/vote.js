const express = require("express");
const router = express.Router();
const { initializeDatabase } = require('../helpers/dbHelpers');
const { ObjectId } = require("mongodb");

router.post("/:id/:score", async (req, res) => {
  const id = req.params.id;
  const score = parseInt(req.params.score);

  if (score !== -1 && score !== 1) {
    res.status(400).send(`Invalid vote of ${score}`);
    return;
  }

  try {
    const collection = await initializeDatabase();
    const map = await collection.findOne({ _id: new ObjectId(id) });

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
    await collection.updateOne({ _id: new ObjectId(id) }, {
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

module.exports = router;
