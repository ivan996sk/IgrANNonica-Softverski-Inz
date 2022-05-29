from cmath import nan
from enum import unique
from itertools import count
import pandas as pd
from sklearn import datasets
import tensorflow as tf
import keras
import numpy as np
import csv
import json
import h5py
import sklearn.metrics as sm
from statistics import mode
from typing_extensions import Self
from copyreg import constructor
from flask import request, jsonify, render_template
from sklearn.preprocessing import LabelEncoder
from sklearn.preprocessing import OrdinalEncoder
import category_encoders as ce
from sklearn.preprocessing import StandardScaler
from sklearn.model_selection import train_test_split
from dataclasses import dataclass
import statistics as s
from sklearn.metrics import roc_auc_score

def returnColumnsInfo(dataset):
    dict=[]
    datafront=dataset.copy()
    svekolone=datafront.columns
    kategorijskekolone=datafront.select_dtypes(include=['object']).columns
    allNullCols=0
    for kolona in svekolone:
        if(kolona in kategorijskekolone):
            uniquevalues=datafront[kolona].unique()
            mean=0
            median=0
            min=0
            max=0
            nullCount=datafront[kolona].isnull().sum()
            if(nullCount>0):
                allNullCols=allNullCols+1
            frontreturn={'columnName':kolona,
                        'isNumber':False,
                        'uniqueValues':uniquevalues.tolist(),
                        'mean':float(mean),
                        'median':float(median),
                        'numNulls':float(nullCount),
                        'min':min,
                        'max':max
            }
            dict.append(frontreturn)
        else:
            mean=datafront[kolona].mean()
            median=s.median(datafront[kolona])
            nullCount=datafront[kolona].isnull().sum()
            min=min(datafront[kolona])
            max=max(datafront[kolona])
            if(nullCount>0):
                allNullCols=allNullCols+1
            frontreturn={'columnName':kolona,
                        'isNumber':1,
                        'uniqueValues':[],
                        'mean':float(mean),
                        'median':float(median),
                        'numNulls':float(nullCount),
                        'min':min,
                        'max':max
            }
            dict.append(frontreturn)
        NullRows = datafront[datafront.isnull().any(axis=1)]
        #print(NullRows)
        #print(len(NullRows))
        allNullRows=len(NullRows)

    return {'columnInfo':dict,'allNullColl':allNullCols,'allNullRows':allNullRows}

@dataclass
class TrainingResultClassification:
    accuracy: float
    precision: float
    recall: float
    tn: float
    fp: float
    fn: float
    tp: float
    specificity: float
    f1: float
    logloss: float
    fpr: float
    tpr: float
    metrics: dict
'''
@datasets
class TrainingResultRegression:
    mse: float
    mae: float
    mape: float
    rmse: float

@dataclass
class TrainingResult:
    metrics: dict
'''
def train(dataset, paramsModel, paramsExperiment, paramsDataset, callback):
    problem_type = paramsModel["type"]
    dataModel = pd.DataFrame()
    dataExperiment = pd.DataFrame()
    for col in paramsModel["inputColumns"]:
        dataModel[col]=dataset[col]
    for col in paramsExperiment["inputColumns"]:
        dataExperiment[col]=dataset[col]

    print(dataModel.head())
    output_column_model = paramsModel["columnToPredict"]
    output_column_experiment = paramsExperiment["outputColumn"]
    dataModel[output_column_model] = dataset[output_column_model]
    #
    # Brisanje null kolona / redova / zamena
    #nullreplace=[
    #    {"column":"Embarked","value":"C","deleteRow":false,"deleteCol":true},
    #    {"column": "Cabin","value":"C123","deleteRow":"0","deleteCol":"0"}]
        
    null_value_options = paramsModel["nullValues"]
    null_values_replacers = paramsModel["nullValuesReplacers"]
    
    if(null_value_options=='replace'):
        print("replace null") # TODO
    elif(null_value_options=='delete_rows'):
        dataModel=dataModel.dropna()
    elif(null_value_options=='delete_columns'):
        dataModel=dataModel.dropna()
    #
    #print(data.isnull().any())
    #
    # Brisanje kolona koje ne uticu na rezultat
    #
    num_rows=dataModel.shape[0]
    for col in dataModel.columns:
        if((dataModel[col].nunique()==(num_rows)) and (dataModel[col].dtype==np.object_)):
            dataModel.pop(col)
    #
    # Enkodiranje
    # https://www.analyticsvidhya.com/blog/2020/08/types-of-categorical-data-encoding/
    #
    encoding=paramsModel["encoding"]
    if(encoding=='label'):
        encoder=LabelEncoder()
        for col in dataModel.columns:
            if(dataModel[col].dtype==np.object_):
                dataModel[col]=encoder.fit_transform(dataModel[col])
    elif(encoding=='onehot'):
        category_columns=[]
        for col in dataModel.columns:
            if(dataModel[col].dtype==np.object_):
                category_columns.append(col)
        dataModel=pd.get_dummies(dataModel, columns=category_columns, prefix=category_columns)
    elif(encoding=='ordinal'):
        encoder = OrdinalEncoder()
        for col in dataModel.columns:
            if(dataModel[col].dtype==np.object_):
                dataModel[col]=encoder.fit_transform(dataModel[col])
 
    elif(encoding=='hashing'):
        category_columns=[]
        for col in dataModel.columns:
            if(dataModel[col].dtype==np.object_):
                category_columns.append(col)
        encoder=ce.HashingEncoder(cols=category_columns, n_components=len(category_columns))
        encoder.fit_transform(dataModel)
    elif(encoding=='binary'):
        category_columns=[]
        for col in dataModel.columns:
            if(dataModel[col].dtype==np.object_):
                category_columns.append(col)
        encoder=ce.BinaryEncoder(cols=category_columns, return_df=True)
        encoder.fit_transform(dataModel)
       
    elif(encoding=='baseN'):
        category_columns=[]
        for col in dataModel.columns:
            if(dataModel[col].dtype==np.object_):
                category_columns.append(col)
        encoder=ce.BaseNEncoder(cols=category_columns, return_df=True, base=5)
        encoder.fit_transform(dataModel)
    #
    # Input - output
    #
    x_columns = []
    for col in dataModel.columns:
        if(col!=output_column_model):
            x_columns.append(col)
    x = dataModel[x_columns].values
    y = dataModel[output_column_model].values
    print(x_columns)
    print(x)
    #
    # Podela na test i trening skupove
    #
    test=paramsModel["randomTestSetDistribution"]
    randomOrder = paramsModel["randomOrder"]
    if(randomOrder):
        random=123
    else:
        random=0
    x_train, x_test, y_train, y_test = train_test_split(x, y, test_size=0.5,random_state=random)
    #
    # Skaliranje vrednosti
    #
    '''
    scaler=StandardScaler()
    scaler.fit(x_train)
    x_test=scaler.transform(x_test)
    x_train=scaler.transform(x_train)
    '''
    
    #
    # Treniranje modela
    #
    #  
    hidden_layer_neurons = paramsModel["hiddenLayerNeurons"]

    if(problem_type=='multi-klasifikacioni'):
        func=paramsModel['hiddenLayerActivationFunctions']
        output_func = paramsModel["outputLayerActivationFunction"]
        optimizer = paramsModel["optimizer"]
        metrics=paramsModel['metrics']
        loss_func=paramsModel["lossFunction"]
        batch_size=paramsModel["batchSize"]
        epochs=paramsModel["epochs"]
        inputDim=len(dataModel.columns) - 1
        '''
        classifier=tf.keras.Sequential()
        
        classifier.add(tf.keras.layers.Dense(units=len(dataModel.columns),input_dim=inputDim))#input layer
        
        for f in func:#hidden layers
            classifier.add(tf.keras.layers.Dense(hidden_layer_neurons,activation=f))
        
        numberofclasses=len(output_column_model.unique())
        classifier.add(tf.keras.layers.Dense(numberofclasses,activation=output_func))#output layer
        '''
        model=tf.keras.Sequential()
        model.add(tf.keras.layers.Dense(1,input_dim=x_train.shape[1]))#input layer
        model.add(tf.keras.layers.Dense(1, activation='sigmoid'))
        model.add(tf.keras.layers.Dense(len(output_column_model.unique())+1, activation='softmax'))
        classifier.compile(optimizer=optimizer, loss=loss_func,metrics=metrics)
        
        history=classifier.fit(x_train, y_train, batch_size=batch_size, epochs=epochs, callbacks=callback(x_test, y_test))
    else:    
        classifier=tf.keras.Sequential()
        
        for func in paramsModel["hiddenLayerActivationFunctions"]:
            layers = paramsModel["hiddenLayers"]
            for numNeurons in paramsModel["hiddenLayerNeurons"]:
                classifier.add(tf.keras.layers.Dense(units=numNeurons,activation=func))
        output_func = paramsModel["outputLayerActivationFunction"]

        if(problem_type!="regresioni"):
            classifier.add(tf.keras.layers.Dense(units=1,activation=output_func))
        else:
            classifier.add(tf.keras.layers.Dense(units=1))

        optimizer = paramsModel["optimizer"]
        metrics=paramsModel['metrics']
        loss_func=paramsModel["lossFunction"]
        classifier.compile(optimizer=optimizer, loss=loss_func,metrics=metrics)
        batch_size = paramsModel["batchSize"]
        epochs = paramsModel["epochs"]
        history=classifier.fit(x_train, y_train, batch_size=batch_size, epochs=epochs, callbacks=callback(x_test, y_test), validation_split=0.2) # TODO params["validationSplit"]
    #
    # Test
    #
    model_name = paramsModel['_id']
    #y_pred=classifier.predict(x_test)
    if(problem_type == "regresioni"):
        y_pred=classifier.predict(x_test)
        print(classifier.evaluate(x_test, y_test))
    elif(problem_type == "binarni-klasifikacioni"):    
        y_pred=classifier.predict(x_test)
        y_pred=(y_pred>=0.5).astype('int')
    elif(problem_type=='multi-klasifikacioni'):
        y_pred=classifier.predict(x_test)
        y_pred=np.argmax(y_pred,axis=1)

    y_pred=y_pred.flatten()
    result=pd.DataFrame({"Actual":y_test,"Predicted":y_pred})
    classifier.save("temp/"+model_name, save_format='h5')
    # ROC multi-klasifikacioni
    def roc_auc_score_multiclass(actual_class, pred_class, average = "macro"):
    
        #creating a set of all the unique classes using the actual class list
        unique_class = set(actual_class)
        roc_auc_dict = {}
        for per_class in unique_class:
            
            #creating a list of all the classes except the current class 
            other_class = [x for x in unique_class if x != per_class]

            #marking the current class as 1 and all other classes as 0
            new_actual_class = [0 if x in other_class else 1 for x in actual_class]
            new_pred_class = [0 if x in other_class else 1 for x in pred_class]

            #using the sklearn metrics method to calculate the roc_auc_score
            roc_auc = roc_auc_score(new_actual_class, new_pred_class, average = average)
            roc_auc_dict[per_class] = roc_auc

        return roc_auc_dict
    #
    # Metrike
    #
    print("HELLO???")
    print(result)
    print("HELLO???")
    if(problem_type=="binarni-klasifikacioni"):
        accuracy = float(sm.accuracy_score(y_test,y_pred))
        precision = float(sm.precision_score(y_test,y_pred))
        recall = float(sm.recall_score(y_test,y_pred))
        tn, fp, fn, tp = sm.confusion_matrix(y_test,y_pred).ravel()
        specificity = float(tn / (tn+fp))
        f1 = float(sm.f1_score(y_test,y_pred))
        fpr, tpr, _ = sm.roc_curve(y_test,y_pred)
        logloss = float(sm.log_loss(y_test, y_pred))
        metrics= {"accuracy" : accuracy,
            "precision" : precision,
            "recall" : recall,
            "specificity" : specificity,
            "f1" : f1,
            "tn" : float(tn),
            "fp" : float(fp),
            "fn" : float(fn),
            "tp" : float(tp),
            "fpr" : fpr.tolist(),
            "tpr" : tpr.tolist(),
            "logloss" : logloss
            }
    elif(problem_type=="regresioni"):
        # https://www.analyticsvidhya.com/blog/2021/05/know-the-best-evaluation-metrics-for-your-regression-model/
        mse = float(sm.mean_squared_error(y_test,y_pred))
        mae = float(sm.mean_absolute_error(y_test,y_pred))
        mape = float(sm.mean_absolute_percentage_error(y_test,y_pred))
        rmse = float(np.sqrt(sm.mean_squared_error(y_test,y_pred)))
        rmsle = float(np.sqrt(sm.mean_squared_error(y_test, y_pred)))
        r2 = float(sm.r2_score(y_test, y_pred))
        # n - num of observations
        # k - num of independent variables
        n = 40
        k = 2
        adj_r2 = float(1 - ((1-r2)*(n-1)/(n-k-1)))
        metrics= {"mse" : mse,
            "mae" : mae,
            "mape" : mape,
            "rmse" : rmse,
            "rmsle" : rmsle,
            "r2" : r2,
            "adj_r2" : adj_r2
            }
    elif(problem_type=="multi-klasifikacioni"):
        
        cr=sm.classification_report(y_test, y_pred)
        cm=sm.confusion_matrix(y_test,y_pred)
        # https://www.kaggle.com/code/nkitgupta/evaluation-metrics-for-multi-class-classification/notebook
        accuracy=metrics.accuracy_score(y_test, y_pred)
        macro_averaged_precision=metrics.precision_score(y_test, y_pred, average = 'macro')
        micro_averaged_precision=metrics.precision_score(y_test, y_pred, average = 'micro')
        macro_averaged_recall=metrics.recall_score(y_test, y_pred, average = 'macro')
        micro_averaged_recall=metrics.recall_score(y_test, y_pred, average = 'micro')
        macro_averaged_f1=metrics.f1_score(y_test, y_pred, average = 'macro')
        micro_averaged_f1=metrics.f1_score(y_test, y_pred, average = 'micro')
        roc_auc_dict=roc_auc_score_multiclass(y_test, y_pred)


    # TODO upload trenirani model nazad na backend
    #return TrainingResult(metrics)


def manageH5(datain,params,h5model):
    dataset=datain.copy()
    problem_type = params["type"]
    data = pd.DataFrame()
    for col in params["inputColumns"]:
        data[col]=dataset[col]
    output_column_model = params["columnToPredict"]
    data[output_column_model] = dataset[output_column_model]
    #
    # Brisanje null kolona / redova / zamena
    #nullreplace=[
    #    {"column":"Embarked","value":"C","deleteRow":false,"deleteCol":true},
    #    {"column": "Cabin","value":"C123","deleteRow":"0","deleteCol":"0"}]
        
    null_value_options = params["nullValues"]
    null_values_replacers = params["nullValuesReplacers"]
    
    if(null_value_options=='replace'):
        print("replace null") # TODO
    elif(null_value_options=='delete_rows'):
        data=data.dropna()
    elif(null_value_options=='delete_columns'):
        data=data.dropna()
    #
    #print(data.isnull().any())
    #
    # Brisanje kolona koje ne uticu na rezultat
    #
    num_rows=data.shape[0]
    for col in data.columns:
        if((data[col].nunique()==(num_rows)) and (data[col].dtype==np.object_)):
            data.pop(col)
    #
    # Enkodiranje
    # https://www.analyticsvidhya.com/blog/2020/08/types-of-categorical-data-encoding/
    #
    encoding=params["encoding"]
    if(encoding=='label'):
        encoder=LabelEncoder()
        for col in data.columns:
            if(data[col].dtype==np.object_):
                data[col]=encoder.fit_transform(data[col])
    elif(encoding=='onehot'):
        category_columns=[]
        for col in data.columns:
            if(data[col].dtype==np.object_):
                category_columns.append(col)
        data=pd.get_dummies(data, columns=category_columns, prefix=category_columns)
    elif(encoding=='ordinal'):
        encoder = OrdinalEncoder()
        for col in data.columns:
            if(data[col].dtype==np.object_):
                data[col]=encoder.fit_transform(data[col])

    elif(encoding=='hashing'):
        category_columns=[]
        for col in data.columns:
            if(data[col].dtype==np.object_):
                category_columns.append(col)
        encoder=ce.HashingEncoder(cols=category_columns, n_components=len(category_columns))
        encoder.fit_transform(data)
    elif(encoding=='binary'):
        category_columns=[]
        for col in data.columns:
            if(data[col].dtype==np.object_):
                category_columns.append(col)
        encoder=ce.BinaryEncoder(cols=category_columns, return_df=True)
        encoder.fit_transform(data)
    
    elif(encoding=='baseN'):
        category_columns=[]
        for col in data.columns:
            if(data[col].dtype==np.object_):
                category_columns.append(col)
        encoder=ce.BaseNEncoder(cols=category_columns, return_df=True, base=5)
        encoder.fit_transform(data)
    #
    # Input - output
    #
    x_columns = []
    for col in data.columns:
        if(col!=output_column_model):
            x_columns.append(col)
    x = data[x_columns].values
    y = data[output_column_model].values


    y_pred=h5model.predict_classes(x)