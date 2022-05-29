from copyreg import constructor
import flask
from flask import request, jsonify, render_template
from sklearn.preprocessing import LabelEncoder
import tensorflow as tf
import pandas as pd
import keras
import csv
import json
import mlservice
import h5py
from mlservice import unositok


app = flask.Flask(__name__)
app.config["DEBUG"] = True


@app.route('/', methods = ['GET', 'POST'])
def index():
    return render_template('index.html')

@app.route('/data', methods = ['GET', 'POST'])
def data():
    if request.method == 'POST':
        print(request.json['filepath'])
        f = request.json['filepath']

        data1 = pd.read_csv(f)

        d2=request.json['filepath2']
        data2=pd.read_csv(d2)
        
        m=request.json['modelpath']
        model=tf.keras.models.load_model(m)
        
        #print(data)
        return unositok(data1,data2,request.json,model)
app.run()