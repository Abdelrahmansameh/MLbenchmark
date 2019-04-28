from flask import Flask, request
import json
app = Flask(__name__)



@app.route('/hello')
def hello():
	return json.dumps(["Hello mistress sexy bunny"])
	
@app.route('/getmove')
def getmove():
	'''Put the input here, direction = 1 means right, direction = -1 means left
	jump = 0 means no jump, jump =1 means jump'''
	return json.dumps({'direction':1, 'jump':1})

@app.route('/testpost', methods=['POST'])
def testpost():
	''' This will give you the game information
	its a json array, element 0 is the player info,
	it has playerx, playery, and grounded (touching the
	ground or not.
	element 1 is the platfoms position. to access the array,
	its info[1]["Items"], and its an array of length 10. 
	for each element in the array, you have the xy coordinates 
	of each platform, in the form ULx eg. 
	If they are 0, the platform doesn't exist'''
	info = request.get_json()
	
	return json.dumps({"result":"success"})
	
def index():

  return 'Server Works!'

  

@app.route('/greet')

def say_hello():

  return 'Hello from Server'