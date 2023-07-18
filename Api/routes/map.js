const express = require("express");
const router = express.Router();
const { initializeDatabase } = require('../helpers/dbHelpers');
const { ObjectId } = require("mongodb");

router.use(express.json()); // Parse JSON data

router.get("/:id", async (req, res) => {
  const { id } = req.params;

  try {
    const collection = await initializeDatabase();
    const mapData = await collection.findOne({ _id: new ObjectId(id) });
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

module.exports = router;
