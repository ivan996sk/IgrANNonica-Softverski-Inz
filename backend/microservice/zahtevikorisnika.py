from sklearn.preprocessing import LabelEncoder
import tensorflow as tf
import pandas as pd
import keras

### 1)Ucitavanje vrednosti
#print(1)
data=pd.read_csv('titanic.csv')

### U promenjivoj kolone nalaze se nazivi svih kolona seta podataka
kolone=data.columns
print(kolone[1])
print(data[kolone[1]].isnull().sum())
print(data[kolone[1]].head(10))

### 2)Proveravanje svih kolona za null vrednosti i popunjavanje medijanom
'''
print(2)
for i in range(len(kolone)):
    if(data[kolone[i]].isnull().sum()>0):
        print(data[kolone[i]].isnull().sum())
        print(2.1)
        podaci=data[kolone[i]].copy()
        print(2.2)
        prosek=podaci.mean()
        print(prosek)
        print(2.3)
        data[kolone[i]].fillna(prosek)
'''
### 3)Pronalazak kategorijskih promenljivih
#print(3)
print(data[kolone[1]].dtypes)
### 4)izbor tipa enkodiranja
#print(4)
enc=input("TIP ENKODIRANJA")
### 5)Enkodiranje svih kategorijskih promenjivih label-encode metodom
#print(5)
if(enc=='label'):
    from sklearn.preprocessing import LabelEncoder
    encoder=LabelEncoder()
    for k in range(len(kolone)):
        if(data[kolone[k]].dtype=='object'):
           data[kolone[k]]=encoder.fit_transform(data[kolone[k]])
### 6)Enkodiranje svoh kategorijskih promenjivih onehot metodom

elif(enc=='onehot'):
    #print(6)
    kategorijskekolone=[]
    for k in range(len(kolone)):
        if(data[kolone[k]].dtype=='object'):
            ###U kategorijske kolone smestaju se nazivi svih kolona sa kategorijskim podacima
            kategorijskekolone.append(kolone[k])
    
    print(kategorijskekolone)
    ### Enkodiranje 
    data=pd.get_dummies(data,columns=kategorijskekolone,prefix=kategorijskekolone)
    print(data.head(10))

### 7)Podela skupa na skup za trening i skup za testiranje
###def prefiks(s):
#
# 
#print(7)

predvidetikol=input("UNETI NAZIV KOLONE ČIJU VREDNOST TREBA PREDVIDETI")
xk=[]
yk=[]
for k in range(len(kolone)):
        if(kolone[k]!=predvidetikol):
            ###U xk se smestaju nazivi kolona cije vrednosti nije potrebno predvideti !!!Prefiks one-hot!!!
            xk.append(kolone[k])
            ###U yk treba smestiti sve nazive kolona cije vrednosti treba predvideti, !!!ukoliko je radjeno one-hot enkodiranje, naziv kolone je promenjen, ima prefiks!!! 
            ###!!!FUNKCIJA ZA PREFIKS!!!
            ###ili odrediti x i y, pa nakon toga izvrsiti enkodiranje???
        '''elif(kolone[k]==predvidetikol):
            yk.append(kolone[k])
        '''
###Podela na x i y
###Dodavanje vrednosti u x
x=data[xk].values
###Dodavanje vrednosti u y, samo za label enkodiranje, bez prefiksa
y=data[predvidetikol].values

#print(data.head(10))
print(data[xk].head(10))
print(data[predvidetikol].head(10))

###Unos velicina za trening i test skup
trening=int(input('UNETI VELIČINU TRENING SKUPA'))
#test=int(input("UNETI VELICINU TESTNOG SKUPA"))

###Provera unetih velicina
if(trening<0 or trening>100):
    print("POGREŠAN UNOS VELIČINE SKUPA ZA TRENING")
if(trening>1):
    trening=trening/100
'''
if(test<0 or test>100):
    print("POGREŠAN UNOS VELIČINE SKUPA ZA TEST")
if(trening>1 and test>1):
    trening=trening/100
    test=test/100
'''
###Da li korisnik zeli nasumicno rasporedjivanje podataka?
nasumicno=input("DA LI ŽELITE NASUMIČNO RASPOREDJIVANJE PODATAKA U TRENING I TEST SKUP?")
###!!!Dugme za nasumici izbor
if(nasumicno=='da'):
    random=50
else:
    random=0


from sklearn.model_selection import train_test_split
x_train,x_test,y_train,y_test=train_test_split(x,y,train_size=trening,random_state=random)

### 8)Skaliranje podataka
from sklearn.preprocessing import StandardScaler
scaler=StandardScaler()
scaler.fit(x_train)
x_test=scaler.transform(x_test)
x_train=scaler.transform(x_train)

#####ZAVRSENA PRIPREMA PODATAKA#####

#####OBUCAVANJE MODELA#####

### 9)Inicijalizacija vestacke neuronske mreze

classifier=tf.keras.Sequential()

### 10)Dodavanje prvog,ulaznog sloja
aktivacijau=input("UNETI ŽELJENU AKTIVACIONU FUNKCIJU ULAZNOG SLOJA")
brojnu=int(input("UNETI BROJ NEURONA ULAZNOG SLOJA"))
classifier.add(tf.keras.layers.Dense(units=brojnu,activation=aktivacijau,input_dim=x_train.shape[1]))

### 11)Dodavanje drugog, skrivenog sloja
aktivacijas=input("UNETI ŽELJENU AKTIVACIONU FUNKCIJU SKRIVENOG SLOJA")
brojns=int(input("UNETI BROJ NEURONA SKRIVENOG SLOJA"))
classifier.add(tf.keras.layers.Dense(units=brojns,activation=aktivacijas))

### 12) Dodavanje treceg, izlaznog sloja
aktivacijai=input("UNETI ŽELJENU AKTIVACIONU FUNKCIJU IZLAZNOG SLOJA")
classifier.add(tf.keras.layers.Dense(units=1,activation=aktivacijai))


### 13) Kompajliranje neuronske mreze
gubici=input("UNETI FUNKCIJU OBRADE GUBITAKA")
optimizator=input("UNETI ŽELJENI OPTIMIZATOR")
classifier.compile(optimizer=optimizator, loss=gubici,metrics = ['accuracy'])

### 14) 
uzorci=int(input("UNETI KOLIKO UZORAKA ĆE BITI UNETO U ISTO VREME"))
classifier.fit(x_train,y_train,batch_size=uzorci,epochs=50)

### 15) Predvidjanje
y_pred=classifier.predict(x_test)

print(y_pred)



    
