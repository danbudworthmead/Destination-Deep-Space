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

    // do some checking here on levelData

    const level = {
        date: new Date().toJSON(),
        id: await collection.countDocuments(),
        name: levelData.name,
        props: levelData.props,
        votes: {
            up: 0, //getRandomInt(100),
            down: 0, //getRandomInt(100),
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
app.post("/vote/:id/:score", async (req, res) => {
    const id = parseInt(req.params.id);
    const score = parseInt(req.params.score);

    if (!(score == -1 || score == 1)) {
        // invalid vote
        res.status(400).send(`Invalid vote of ${score}`);
        return;
    }

    const map = await collection.findOne({id: id});
    if (map == undefined) {
        res.status(400).send(`Invalid map id of ${id}`);
        return;
    }

    if (score == 1) {
        map.votes.up++;
    } else if (score == -1) {
        map.votes.down++;
    }

    const options = { upsert: false };

    // update the doc
    await collection.updateOne({id: id}, {"$set": {
        votes: map.votes,
        completions: ++map.completions,
    }}, options);

    // send the response
    res.status(200).send(`Score of ${id} is now ${map.votes.up - map.votes.down}`);
});

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

const port = 30674;
app.listen(port);
client.close();