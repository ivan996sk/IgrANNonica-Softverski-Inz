import numpy as np
from tensorflow.keras.datasets import boston_housing
from sklearn.preprocessing import StandardScaler
from tensorflow.keras.models import load_model

x_new = np.random.rand(X_train.shape[1])
x_new = x_new.reshape(1,-1)
# x_new.shape
y_pred = model.predict(x_new)
if y_pred < 0.5:
    print('0')
else:
    print('1')

(X_train, y_train), (X_test, y_test) = boston_housing.load_data()

X_train.shape
X_test.shape
X_train[0]
y_test

scaler = StandardScaler()
scaler.fit(X_train)
X_train = scaler.transform(X_train)
X_test = scaler.transform(X_test)

X_train[0]
X_test

model = Sequential()
model.add(Dense(input_dim=X_train.shape[1], units=100, activation='relu'))
# model.add(Dense(input_dim=X_train.shape[1], units=50, activation='relu'))
model.add(Dense(units=1))
model.summary()

model.compile(optimizer='adam', loss='mse', metrics=['mae'])
history = model.fit(X_train, y_train, batch_size=32, epochs=20, validation_split=0.2)

plt.plot(history.epoch, history.history['loss'])
plt.plot(history.epoch, history.history['val_loss'])

plt.plot(history.epoch, history.history['mae'])
plt.plot(history.epoch, history.history['val_mae'])

model.evaluate(X_test, y_test)
model.save('models/boston.h5')
old_model = load_model('models/boston.h5')

x_new = np.random.rand(X_train.shape[1])
x_new = x_new.reshape(1, -1)
x_new.shape
# Za nove podatke koristi se predict tako da se dobije predvidjena vrednost za y
old_model.predict(x_new)