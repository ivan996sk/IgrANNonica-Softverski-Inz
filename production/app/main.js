const express = require('express');
const cors = require('cors');
const path = require('path');

const app = express();

const port = 10091;

app.use(cors());
app.use(express.static(path.join(__dirname, './dist')));

app.get(['/', '', '*'], (req, res) => {
    res.sendFile(path.join(__dirname, './dist/index.html'));
});

app.listen(port, () => {
    console.log(`Listening on port ${port}`);
});