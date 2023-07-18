const { MongoClient } = require("mongodb");
const BodyParser = require('body-parser');

const mongodbConnString = `mongodb+srv://danbudworthmead:cS38qN2AYZqM3cEz@destinationdeepspace.wmvclne.mongodb.net/?retryWrites=true&w=majority`;

const client = new MongoClient(mongodbConnString);
client.connect();

const database = client.db('CommunityLevelsDB');
const collection = database.collection('CommunityLevelsColl');

const app = require("express")();
app.use(BodyParser.json());
app.use(BodyParser.urlencoded({ extended: true }));

/*
 * Submit a map
 */
app.post("/map", async (req, res) => {
    console.log(`${req.hostname} used POST/map`);

    const levelData = req.body;
    const level = {
        date: new Date().toJSON(),
        id: await collection.countDocuments(),
        name: levelData.name,
        props: levelData.props,
        votes: {
            up: getRandomInt(100),
            down: getRandomInt(100),
        },
        completions: 0,
    };

    await collection.insertOne(level);
    console.log(`Added map ${level.name}`);

    res.status(200).send();
});

/*
 * Get a map with a specific id
 */
app.get("/map/:id", async (req, res) => {
    const { id } = req.params;
    console.log(`${req.hostname} used GET/map/${id}`);

    var query = {id: parseInt(id)};
    console.log(query);
    var mapData = await collection.findOne(query);
    console.log(mapData);
    res.status(200).send(mapData);
});

/*
 * Get a list of the top rated maps of all time 
 */
app.get("/all", async (req, res) => {
    const cursor = collection.find();
    const maps = [];
    for await (const map of cursor) {
        delete map.props;
        map.score = map.votes.up - map.votes.down;
        delete map.votes;
        maps.push(map);
    }

    // Sort the maps by date in descending order (newest to oldest)
    maps.sort((mapA, mapB) => new Date(mapB.score) - new Date(mapA.score));

    console.log(maps);
    res.status(200).send(maps);
});

/*
 * Vote on a map
 */
app.post("/vote/:score", async (req, res) => {
    const { score } = req.params;
});

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

const port = 30674;
app.listen(port);
client.close();