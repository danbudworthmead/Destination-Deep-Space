const express = require("express");
const router = express.Router();
const { addMapToCollection, initializeDatabase } = require('../helpers/dbHelpers');

router.use(express.json()); // Parse JSON data
router.post("/", async (req, res) => {
  console.log(`${req.hostname} used POST/map ${JSON.stringify(req.body)}`);
  const levelData = req.body;

  if (!levelData) {
    res.status(400).send("body was undefined");
    return;
  }

  if (!levelData.name) {
    res.status(400).send("name was undefined");
    return;
  }

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

router.get("/:id", async (req, res) => {
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

module.exports = router;
