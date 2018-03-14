REM Enable the CNTK backend first https://keras.io/backend/
python createmodel.py

REM I had to hack the keras2_parser to remove the clear_session call cause it didn't work
python -m mmdnn.conversion._script.convertToIR -f keras -d converted -n model.json -w model.h5
python -m mmdnn.conversion._script.IRToCode -f cntk -d converted_cntk.py -n converted.pb -w converted.npy
python -m mmdnn.conversion.examples.cntk.imagenet_test -n converted_cntk -w converted.npy --dump model.dnn