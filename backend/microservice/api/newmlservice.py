from cmath import nan
from enum import unique
from itertools import count
import os
from sys import breakpointhook
import pandas as pd
from sklearn import datasets, multiclass
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
from sklearn.preprocessing import LabelEncoder, MinMaxScaler
from sklearn.preprocessing import OrdinalEncoder
import category_encoders as ce
from sklearn.preprocessing import StandardScaler
from sklearn.model_selection import train_test_split
from dataclasses import dataclass
import statistics as s
from sklearn.metrics import roc_auc_score
import matplotlib.pyplot as plt
#from ann_visualizer.visualize import ann_viz;
def returnColumnsInfo(dataset):
    dict=[]
    
    datafront=dataset.copy()
    dataMatrix=dataset.copy()
    
   
    svekolone=datafront.columns
    kategorijskekolone=datafront.select_dtypes(include=['object']).columns

    allNullCols=0
    rowCount=datafront.shape[0]#ukupan broj redova
    colCount=len(datafront.columns)#ukupan broj kolona

    for kolona in svekolone:
        if(kolona in kategorijskekolone):
            encoder=LabelEncoder()
            dataMatrix[kolona]=encoder.fit_transform(dataMatrix[kolona])

    #print(dataMatrix.dtypes)
    cMatrix=dataMatrix.corr()

    for kolona in svekolone:
        if(kolona in kategorijskekolone):
            unique=datafront[kolona].value_counts()
            uniquevalues=[]
            uniquevaluescount=[]
            uniquevaluespercent=[]
            for val, count in unique.iteritems():
                if(val):
                    uniquevalues.append(val)
                    uniquevaluescount.append(count)
                    percent=count/rowCount
                    uniquevaluespercent.append(percent)
            #print(uniquevalues)
            #print(uniquevaluescount)
            mean=0
            median=0
            minimum=0
            maximum=0
            q1=0
            q3=0
            nullCount=datafront[kolona].isnull().sum()
            if(nullCount>0):
                allNullCols=allNullCols+1
            frontreturn={
                        'columnName':kolona,
                        'isNumber':False,
                        'uniqueValues':uniquevalues,
                        'uniqueValuesCount':uniquevaluescount,
                        'uniqueValuesPercent':uniquevaluespercent,
                        'mean':float(mean),
                        'median':float(median),
                        'numNulls':int(nullCount),
                        'min':float(minimum),
                        'max':float(maximum),
                        'q1':float(q1),
                        'q3':float(q3),

            }
            dict.append(frontreturn)
        else:
            minimum=min(datafront[kolona])
            maximum=max(datafront[kolona])
            mean=datafront[kolona].mean()
            median=s.median(datafront[kolona].copy().dropna())
            q1= np.percentile(datafront[kolona].copy().dropna(), 25)
            q3= np.percentile(datafront[kolona].copy().dropna(), 75)
            nullCount=datafront[kolona].isnull().sum()
            if(nullCount>0):
                allNullCols=allNullCols+1

            #pretvaranje u kategorijsku
            datafront = datafront.astype({kolona: str})
            #print(datafront.dtypes)
            unique=datafront[kolona].value_counts()
            uniquevaluesn=[]
            uniquevaluescountn=[]
            uniquevaluespercentn=[]
            for val, count in unique.iteritems():
                if(val):
                    uniquevaluesn.append(val)
                    uniquevaluescountn.append(count)
                    percent=count/rowCount
                    uniquevaluespercentn.append(percent)
            frontreturn={
                        'columnName':kolona,
                        'isNumber':1,
                        'uniqueValues':uniquevaluesn,
                        'uniqueValuesCount':uniquevaluescountn,
                        'uniqueValuesPercent':uniquevaluespercentn,
                        'mean':float(mean),
                        'median':float(median),
                        'numNulls':int(nullCount),
                        'min':float(minimum),
                        'max':float(maximum),
                        'q1':float(q1),
                        'q3':float(q3),

            }
            dict.append(frontreturn)
        NullRows = datafront[datafront.isnull().any(axis=1)]
        #print(NullRows)
        #print(len(NullRows))
        allNullRows=len(NullRows)
        #print(cMatrix.to_json(orient='index'))
        #json.loads()['data']
    return {'columnInfo':dict,'allNullColl':int(allNullCols),'allNullRows':int(allNullRows),'rowCount':int(rowCount),'colCount':int(colCount),'cMatrix':json.loads(cMatrix.to_json(orient='split'))['data']}

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

def train(dataset, paramsModel,paramsExperiment,paramsDataset,callback):
    ###UCITAVANJE SETA
    problem_type = paramsModel["type"]
    #print(problem_type)
    data = pd.DataFrame()
    #print(data)
    for col in paramsExperiment["inputColumns"]:
        #print(col)
        if(col!=paramsExperiment["outputColumn"]):
            data[col]=dataset[col]
    output_column = paramsExperiment["outputColumn"]
    data[output_column] = dataset[output_column]
    #print(data)

    ###KATEGORIJSKE KOLONE
    kategorijskekolone=[]
    ###PRETVARANJE NUMERICKIH U KATREGORIJSKE AKO JE KORISNIK TAKO OZNACIO

    columnInfo=paramsDataset['columnInfo']
    columnTypes=paramsExperiment['columnTypes']
    for i in range(len(columnInfo)):
        col=columnInfo[i]
        if(columnTypes[i]=="categorical" and col['columnName'] in paramsExperiment['inputColumns']):
            data[col['columnName']]=data[col['columnName']].apply(str)
            kategorijskekolone.append(col['columnName'])
    #kategorijskekolone=data.select_dtypes(include=['object']).columns
    #print(kategorijskekolone)
    ###NULL
    #null_value_options = paramsExperiment["nullValues"] #
    null_values_replacers = paramsExperiment["nullValuesReplacers"] #{"column":"naziv","opt":"tip promene","value":"vrednost za zamenu"}
    
    #if(null_value_options=='replace'):
        #print("replace null") 
    dict=null_values_replacers
    while(len(dict)>0):
        replace=dict.pop()
        col=replace['column']
        opt=replace['option']

        if(opt=='replace'):
            replacevalue=replace['value']
            data[col]=data[col].fillna(replacevalue)
        
    data=data.dropna()
            
    #print(data)

    #print(data.shape)
    
    #
    # Brisanje kolona koje ne uticu na rezultat
    #
    '''
    num_rows=data.shape[0]
    for col in data.columns:
        if((data[col].nunique()==(num_rows)) and (data[col].dtype==np.object_)):
            data.pop(col)
    #
    '''
    ### Enkodiranje
    encodings=paramsExperiment["encodings"]
    #datafront=dataset.copy()
    #svekolone=datafront.columns
    #kategorijskekolone=datafront.select_dtypes(include=['object']).columns
    for kolonaEncoding in encodings:
        
        kolona = kolonaEncoding["columnName"]
        encoding = kolonaEncoding["encoding"]
    
        if(kolona in kategorijskekolone):
            if(encoding=='label'):
                encoder=LabelEncoder()
                for col in data.columns:
                    if(data[col].dtype==np.object_):
                        data[col]=encoder.fit_transform(data[col])
    
    
            elif(encoding=='onehot'):
                if((len(pd.unique(data[kolona]))>20)or (kolona==output_column)):
                    encoder=LabelEncoder()
                    data[kolona]=encoder.fit_transform(data[kolona])
                
                else:    
                    data=pd.get_dummies(data, columns=kolona, prefix=kolona)

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
        if(col!=output_column):
            x_columns.append(col)
    #print(x_columns)
    x = data[x_columns].values
    y = data[output_column].values

    #
    # Podela na test i trening skupove
    #
    test=paramsModel["randomTestSetDistribution"]
    randomOrder = paramsModel["randomOrder"]
    if(randomOrder):
        random=123
    else:
        random=0
    
    
    #x_train, x_test, y_train, y_test = train_test_split(x, y, test_size=test, random_state=random)
    #print(x_train,x_test)
    x, x_test, y, y_test = train_test_split(x, y, test_size=test, random_state=random, shuffle=True)
    x_train, x_val, y_train, y_val = train_test_split(x, y, test_size=(1.0-paramsModel['validationSize']))
    # Treniranje modela
    #
    #
    ###OPTIMIZATORI
    print(paramsModel['optimizer'])
    if(paramsModel['optimizer']=='Adam'):
        opt=tf.keras.optimizers.Adam(learning_rate=float(paramsModel['learningRate']))

    elif(paramsModel['optimizer']=='Adadelta'):
        opt=tf.keras.optimizers.Adadelta(learning_rate=float(paramsModel['learningRate']))

    elif(paramsModel['optimizer']=='Adagrad'): 
        opt=tf.keras.optimizers.Adagrad(learning_rate=float(paramsModel['learningRate']))

    elif(paramsModel['optimizer']=='Adamax'):
        opt=tf.keras.optimizers.Adamax(learning_rate=float(paramsModel['learningRate']))

    elif(paramsModel['optimizer']=='Nadam'):
        opt=tf.keras.optimizers.Nadam(learning_rate=float(paramsModel['learningRate']))

    elif(paramsModel['optimizer']=='SGD'):
        opt=tf.keras.optimizers.SGD(learning_rate=float(paramsModel['learningRate']))

    if(paramsModel['optimizer']=='SGDMomentum'):
        opt=tf.keras.optimizers.SGD(learning_rate=float(paramsModel['learningRate']))

    elif(paramsModel['optimizer']=='Ftrl'):
        opt=tf.keras.optimizers.Ftrl(learning_rate=float(paramsModel['learningRate']))
    
    elif(paramsModel['optimizer']=='RMSprop'):
        opt=tf.keras.optimizers.RMSprop(learning_rate=float(paramsModel['learningRate']))

    ###REGULARIZACIJA
    #regularisation={'kernelType':'l1 ili l2 ili l1_l2','kernelRate':default=0.01 ili jedna od vrednosti(0.0001,0.001,0.1,1,2,3) ili neka koju je korisnik zadao,'biasType':'','biasRate':'','activityType','activityRate'}
   

    filepath=os.path.join("temp/",paramsExperiment['_id']+"_"+paramsModel['_id']+".h5")
    if(problem_type=='multi-klasifikacioni'):
        #print('multi')
        #print(paramsModel)
        reg=paramsModel['layers'][0]['regularisation']
        regRate=float(paramsModel['layers'][0]['regularisationRate'])
        if(reg=='l1'):
            kernelreg=tf.keras.regularizers.l1(regRate)
            biasreg=tf.keras.regularizers.l1(regRate)
            activityreg=tf.keras.regularizers.l1(regRate)
        elif(reg=='l2'):
            kernelreg=tf.keras.regularizers.l2(regRate)
            biasreg=tf.keras.regularizers.l2(regRate)
            activityreg=tf.keras.regularizers.l2(regRate)

        classifier=tf.keras.Sequential()
        classifier.add(tf.keras.layers.Dense(units=paramsModel['layers'][0]['neurons'], activation=paramsModel['layers'][0]['activationFunction'],input_dim=x_train.shape[1], kernel_regularizer=kernelreg, bias_regularizer=biasreg, activity_regularizer=activityreg))#prvi skriveni + definisanje prethodnog-ulaznog
       
        for i in range(paramsModel['hiddenLayers']-1):#ako postoji vise od jednog skrivenog sloja
            ###Kernel
            reg=paramsModel['layers'][i+1]['regularisation']
            regRate=float(paramsModel['layers'][i+1]['regularisationRate'])
            if(reg=='l1'):
                kernelreg=tf.keras.regularizers.l1(regRate)
                biasreg=tf.keras.regularizers.l1(regRate)
                activityreg=tf.keras.regularizers.l1(regRate)
            elif(reg=='l2'):
                kernelreg=tf.keras.regularizers.l2(regRate)
                biasreg=tf.keras.regularizers.l2(regRate)
                activityreg=tf.keras.regularizers.l2(regRate)

            classifier.add(tf.keras.layers.Dense(units=paramsModel['layers'][i+1]['neurons'], activation=paramsModel['layers'][i+1]['activationFunction'],kernel_regularizer=kernelreg, bias_regularizer=biasreg, activity_regularizer=activityreg))#i-ti skriveni sloj
        
        classifier.add(tf.keras.layers.Dense(units=5, activation=paramsModel['outputLayerActivationFunction']))#izlazni sloj

        

        classifier.compile(loss =paramsModel["lossFunction"] , optimizer =opt, metrics = ['accuracy'])

        history=classifier.fit( x=x_train, y=y_train, epochs = paramsModel['epochs'],batch_size=int(paramsModel['batchSize']),callbacks=callback(x_test, y_test,paramsModel['_id']),validation_data=(x_val, y_val))

        hist=history.history
        #plt.plot(hist['accuracy'])
        plt.plot(history.history['loss'])
        plt.plot(history.history['val_loss'])
        plt.show()
        y_pred=classifier.predict(x_test)
        y_pred=np.argmax(y_pred,axis=1)
        
        scores = classifier.evaluate(x_test, y_test)
        #print("\n%s: %.2f%%" % (classifier.metrics_names[1], scores[1]*100))
        
        '''
        classifier.save(filepath, save_format='h5')
       
        macro_averaged_precision=sm.precision_score(y_test, y_pred, average = 'macro')
        micro_averaged_precision=sm.precision_score(y_test, y_pred, average = 'micro')
        macro_averaged_recall=sm.recall_score(y_test, y_pred, average = 'macro')
        micro_averaged_recall=sm.recall_score(y_test, y_pred, average = 'micro')
        macro_averaged_f1=sm.f1_score(y_test, y_pred, average = 'macro')
        micro_averaged_f1=sm.f1_score(y_test, y_pred, average = 'micro')

        metrics= [
                {"Name":"macro_averaged_precision", "JsonValue":str(macro_averaged_precision)},
                {"Name":"micro_averaged_precision" ,"JsonValue":str(micro_averaged_precision)},
                {"Name":"macro_averaged_recall", "JsonValue":str(macro_averaged_recall)},
                {"Name":"micro_averaged_recall" ,"JsonValue":str(micro_averaged_recall)},
                {"Name":"macro_averaged_f1","JsonValue": str(macro_averaged_f1)},
                {"Name":"micro_averaged_f1", "JsonValue": str(micro_averaged_f1)}
        ]
        '''
        #vizuelizacija u python-u
        #from ann_visualizer.visualize import ann_viz;
        #ann_viz(classifier, title="My neural network")
        
        return filepath,[hist['loss'],hist['val_loss'],hist['accuracy'],hist['val_accuracy'],[],[],[],[]]

    elif(problem_type=='binarni-klasifikacioni'):
        #print('*************************************************************************binarni')
        reg=paramsModel['layers'][0]['regularisation']
        regRate=float(paramsModel['layers'][0]['regularisationRate'])
        if(reg=='l1'):
            kernelreg=tf.keras.regularizers.l1(regRate)
            biasreg=tf.keras.regularizers.l1(regRate)
            activityreg=tf.keras.regularizers.l1(regRate)
        elif(reg=='l2'):
            kernelreg=tf.keras.regularizers.l2(regRate)
            biasreg=tf.keras.regularizers.l2(regRate)
            activityreg=tf.keras.regularizers.l2(regRate)
        classifier=tf.keras.Sequential()
    
        classifier.add(tf.keras.layers.Dense(units=paramsModel['layers'][0]['neurons'], activation=paramsModel['layers'][0]['activationFunction'],input_dim=x_train.shape[1],kernel_regularizer=kernelreg, bias_regularizer=biasreg, activity_regularizer=activityreg))#prvi skriveni + definisanje prethodnog-ulaznog
        
        for i in range(paramsModel['hiddenLayers']-1):#ako postoji vise od jednog skrivenog sloja
            #print(i)
            reg=paramsModel['layers'][i+1]['regularisation']
            regRate=float(paramsModel['layers'][0]['regularisationRate'])
            if(reg=='l1'):
                kernelreg=tf.keras.regularizers.l1(regRate)
                biasreg=tf.keras.regularizers.l1(regRate)
                activityreg=tf.keras.regularizers.l1(regRate)
            elif(reg=='l2'):
                kernelreg=tf.keras.regularizers.l2(regRate)
                biasreg=tf.keras.regularizers.l2(regRate)
                activityreg=tf.keras.regularizers.l2(regRate)
            classifier.add(tf.keras.layers.Dense(units=paramsModel['layers'][i+1]['neurons'], activation=paramsModel['layers'][i+1]['activationFunction']))#i-ti skriveni sloj
        
        classifier.add(tf.keras.layers.Dense(units=1, activation=paramsModel['outputLayerActivationFunction']))#izlazni sloj

        classifier.compile(loss =paramsModel["lossFunction"] , optimizer =opt , metrics = ['accuracy'])

        history=classifier.fit( x=x_train, y=y_train, epochs = paramsModel['epochs'],batch_size=int(paramsModel['batchSize']),callbacks=callback(x_test, y_test,paramsModel['_id']),validation_data=(x_val, y_val))
        hist=history.history
        
        y_pred=classifier.predict(x_test)
        y_pred=(y_pred>=0.5).astype('int')
        
        scores = classifier.evaluate(x_test, y_test)
        #print("\n%s: %.2f%%" % (classifier.metrics_names[1], scores[1]*100))
        # ann_viz(classifier, title="My neural network")
        
        classifier.save(filepath, save_format='h5')
        """
        accuracy = float(sm.accuracy_score(y_test,y_pred))
        precision = float(sm.precision_score(y_test,y_pred))
        recall = float(sm.recall_score(y_test,y_pred))
        tn, fp, fn, tp = sm.confusion_matrix(y_test,y_pred).ravel()
        specificity = float(tn / (tn+fp))
        f1 = float(sm.f1_score(y_test,y_pred))
        fpr, tpr, _ = sm.roc_curve(y_test,y_pred)
        logloss = float(sm.log_loss(y_test, y_pred))
        """

        return filepath,[hist['loss'],hist['val_loss'],hist['accuracy'],hist['val_accuracy'],[],[],[],[]]

    elif(problem_type=='regresioni'):
        reg=paramsModel['layers'][0]['regularisation']
        regRate=float(paramsModel['layers'][0]['regularisationRate'])
        if(reg=='l1'):
            kernelreg=tf.keras.regularizers.l1(regRate)
            biasreg=tf.keras.regularizers.l1(regRate)
            activityreg=tf.keras.regularizers.l1(regRate)
        elif(reg=='l2'):
            kernelreg=tf.keras.regularizers.l2(regRate)
            biasreg=tf.keras.regularizers.l2(regRate)
            activityreg=tf.keras.regularizers.l2(regRate)
        classifier=tf.keras.Sequential()
    
        classifier.add(tf.keras.layers.Dense(units=paramsModel['layers'][0]['neurons'], activation=paramsModel['layers'][0]['activationFunction'],input_dim=x_train.shape[1],kernel_regularizer=kernelreg, bias_regularizer=biasreg, activity_regularizer=activityreg))#prvi skriveni + definisanje prethodnog-ulaznog
        
        for i in range(paramsModel['hiddenLayers']-1):#ako postoji vise od jednog skrivenog sloja
            #print(i)
            reg=paramsModel['layers'][i+1]['regularisation']
            regRate=float(paramsModel['layers'][i+1]['regularisationRate'])
            if(reg=='l1'):
                kernelreg=tf.keras.regularizers.l1(regRate)
                biasreg=tf.keras.regularizers.l1(regRate)
                activityreg=tf.keras.regularizers.l1(regRate)
            elif(reg=='l2'):
                kernelreg=tf.keras.regularizers.l2(regRate)
                biasreg=tf.keras.regularizers.l2(regRate)
                activityreg=tf.keras.regularizers.l2(regRate)

            classifier.add(tf.keras.layers.Dense(units=paramsModel['layers'][i+1]['neurons'], activation=paramsModel['layers'][i+1]['activationFunction'],kernel_regularizer=kernelreg, bias_regularizer=biasreg, activity_regularizer=activityreg))#i-ti skriveni sloj
        
        classifier.add(tf.keras.layers.Dense(units=1))

        classifier.compile(loss =paramsModel["lossFunction"] , optimizer = opt , metrics = ['mae','mse'])

        history=classifier.fit( x=x_train, y=y_train, epochs = paramsModel['epochs'],batch_size=int(paramsModel['batchSize']),callbacks=callback(x_test, y_test,paramsModel['_id']),validation_data=(x_val, y_val))
        hist=history.history
        
        y_pred=classifier.predict(x_test)
        #print(classifier.evaluate(x_test, y_test))
 
        classifier.save(filepath, save_format='h5')
        '''

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
       
        metrics= [
            {"Name":"mse","JsonValue":str(mse)},

            {"Name":"mae","JsonValue":str(mae)},
            {"Name":"mape","JsonValue":str( mape)},
            {"Name":"rmse","JsonValue":str(rmse)},
            {"Name":"rmsle","JsonValue":str(rmsle)},
            {"Name":"r2","JsonValue":str( r2)},
            {"Name":"adj_r2","JsonValue":str(adj_r2)}
            ]
        '''
        return filepath,[hist['loss'],hist['val_loss'],[],[],hist['mae'],hist['val_mae'],hist['mse'],hist['val_mse']]

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
    '''
    if(problem_type=="regresioni"):
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
'''   
def predict(experiment, predictor, model):
    #model.predict()
    # ovo je pre bilo manageH5 
    return "TODO"


def manageH5(dataset,paramsModel,paramsExperiment,paramsDataset,h5model,callback):
    problem_type = paramsModel["type"]
    data = pd.DataFrame()
    for col in paramsExperiment["inputColumns"]:
        if(col!=paramsExperiment["outputColumn"]):
            data[col]=dataset[col]
    output_column = paramsExperiment["outputColumn"]
    data[output_column] = dataset[output_column]

    kategorijskekolone=[]
    columnInfo=paramsDataset['columnInfo']
    columnTypes=paramsExperiment['columnTypes']
    for i in range(len(columnInfo)):
        col=columnInfo[i]
        if(columnTypes[i]=="categorical" and col['columnName'] in paramsExperiment['inputColumns']):
            data[col['columnName']]=data[col['columnName']].apply(str)
            kategorijskekolone.append(col['columnName'])

    null_value_options = paramsExperiment["nullValues"]
    null_values_replacers = paramsExperiment["nullValuesReplacers"]
    
    if(null_value_options=='replace'):
        dict=null_values_replacers
        while(len(dict)>0):
            replace=dict.pop()
            col=replace['column']
            opt=replace['option']
            if(opt=='replace'):
                replacevalue=replace['value']
                data[col]=data[col].fillna(replacevalue)
    elif(null_value_options=='delete_rows'):
        data=data.dropna()
    elif(null_value_options=='delete_columns'):
        data=data.dropna(axis=1)

    encodings=paramsExperiment["encodings"]
    for kolonaEncoding in encodings:
        kolona = kolonaEncoding["columnName"]
        encoding = kolonaEncoding["encoding"]
        if(kolona in kategorijskekolone):
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

    x_columns = []
    for col in data.columns:
        if(col!=output_column):
            x_columns.append(col)
    x2 = data[x_columns]
    x2 = data[x_columns].values
    y2 = data[output_column].values
    #h5model.summary()
    #ann_viz(h5model, title="My neural network")


    history=h5model.fit(x2, y2, epochs = paramsModel['epochs'],batch_size=int(paramsModel['batchSize']),callbacks=callback(x2, y2,paramsModel['_id']))  
    y_pred2=h5model.predict(x2)
    y_pred2=np.argmax(y_pred2,axis=1)
    #y_pred=h5model.predict_classes(x)
    score = h5model.evaluate(x2,y_pred2, verbose=0)
    #print("%s: %.2f%%" % (h5model.metrics_names[1], score[1]*100))
    #print(y_pred2)
    #print( 'done')