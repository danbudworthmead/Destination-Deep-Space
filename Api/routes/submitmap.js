const express = require("express");
const router = express.Router();
const { addMapToCollection, initializeDatabase } = require('../helpers/dbHelpers');

router.use(express.json()); // Parse JSON data
router.post("/", async (req, res) => {
  const levelData = req.body;

  if (!levelData) {
    res.status(400).send("body was undefined");
    return;
  }

  console.log(`levelData.name: ${levelData.name}`);
  console.log(`levelData.name.length: ${levelData.name.length}`);

  // Remove any non-printable characters or leading/trailing whitespace
  const cleanedName = levelData.name.replace(/[^\x20-\x7E]/g, "").trim();

  if (typeof cleanedName !== "string" || cleanedName === "") {
      res.status(400).send("name was undefined or empty");
      return;
  }

  try {
    const collection = await initializeDatabase();
    await addMapToCollection(levelData, collection);
    res.status(200).send();
  } catch (error) {
    console.error("Error adding map:", error);
    res.status(500).send("Internal Server Error");
  }
});

module.exports = router;
