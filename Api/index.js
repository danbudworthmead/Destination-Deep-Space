const express = require("express");
const rateLimit = require('express-rate-limit');

const app = express();
const port = 30674;

app.use(express.json());

// Custom limiter for POST map requests
const mapLimited = rateLimit({
  windowMs: 60 * 60 * 1000, // 1 hour
  max: 6, // Limit to 6 map creations per hour from the same IP
  message: 'Too many map creations recently, please try again later.',
  headers: true,
  keyGenerator: (req) => {
    return req.ip;
  },
});

// Apply the custom limiter only to the POST route
app.use("/map", mapLimited);

// Custom limiter for POST vote requests
const voteLimited = rateLimit({
  windowMs: 60 * 60 * 1000, // 1 hour
  max: 20, // Limit to 20 votes per hour from the same IP
  message: 'Too many votes recently, please try again later.',
  headers: true,
  keyGenerator: (req) => {
    return req.ip;
  },
});

app.use("/vote", voteLimited);

// routes
app.use('/map', require('./routes/map'));
app.use('/vote', require('./routes/vote'));
app.use('/all', require('./routes/all'));

// Error handling middleware
app.use((err, req, res, next) => {
  console.error("Internal Server Error:", err);
  res.status(500).send("Internal Server Error");
});

app.listen(port, () => {
  console.log(`Server listening on port ${port}`);
});
