'''
RUN THESE BEFORE RUNNING SERVER
> set FLASK_ENV=development
> set FLASK_APP=app.py
'''

from flask import Flask, request
import math
import json
import random
app = Flask(__name__)
import os

class Player:
	def __init__(self, env):
		self.x = 0
		self.y = 0
		self.grounded = False
		self.env = env
		env.addPlayer(self)
		self.distanceToJump = 0.2
		self.dead = False
		
	def update(self, x, y, grounded, dead):
		self.x = x
		self.y = y
		self.grounded = grounded 
		self.dead = dead
	def closestRightEdge(self):
		platforms = sorted(self.env.platforms, key = lambda x : self.distanceToX(x.ur[0]) )
		#print(self.env.platforms)
		if platforms == []:
			return math.inf
		#print(self.x) 
		return self.distanceToX(platforms[0].ur[0])

	def distanceToX(self, x):
		return abs(self.x - x)
		
	def nextPlatformLeftEdge(self):
		platforms = sorted(self.env.platforms, key = lambda x: self.distanceToX(x.ul[0]) )
		i = 0
		while platforms[i].ul[0] <= self.x:
			i += 1
		return self.distanceToX(platforms[i])
		
	def nextMove(self):
		''' Return a couple dirction, jump
		Direction = 1 means right, -1 means left
		Jump is a bool
		'''
		if self.env.DoIReset():
			os.system('taskkill /f /t /im MachineLearning.exe')
			os.system('"MachineLearning.exe"')
		return 1,0
		return random.randint(-1 , 1), int(random.random()<=0.5)
		if (self.closestRightEdge() <= self.distanceToJump):
			return 1, 1
		return 1, 0
		
		
	
class Platform:
	def __init__(self, ul, bl, ur, br, env):
		self.ul = ul
		self.bl = bl
		self.ur = ur
		self.br = br
		self.env = env
		env.addPlatform(self)
		
class gameEnv:
	def __init__(self, counter):
		print("Reinitialized game environment")
		self.players = []
		self.platforms = []
		self.counter = counter
		
	def addPlayer(self, player):
		self.players.append(player)

	def addPlatform(self, platform):
		self.platforms.append(platform)
	
	def AllPlayersDead(self):
		ret = True
		for p in self.players:
			if not p.dead:
				ret = False
		return ret
		
	def DoIReset(self):
		return self.AllPlayersDead()

env = gameEnv(0)

@app.route('/hello')
def hello():
	global env
	env = gameEnv(env.counter + 1)
	p = Player(env)
	return "Hello mistress sexy bunny"
	
@app.route('/getmove')
def getmove():
	'''Put the input here, direction = 1 means right, direction = -1 means left
	jump = 0 means no jump, jump =1 means jump'''
	global env
	mv = env.players[0].nextMove()
	#print(mv)
	return json.dumps({'direction':mv[0], 'jump':mv[1]})

@app.route('/sendenv', methods=['POST'])
def sendenv():
	''' This will give you the game information
	its a json array, element 0 is the player info,
	it has playerx, playery, and grounded (touching the
	ground or not.
	element 1 is the platfoms position. to access the array,
	its info[1]["Items"], and its an array of length 10. 
	for each element in the array, you have the xy coordinates 
	of each platform, in the form ULx eg. 
	If they are 0, the platform doesn't exist'''
	global env
	print(env.counter)
	info = request.get_json()
	pinfo =  info[0]
	#print(info)
	p = env.players[0]
	p.update(pinfo["playerx"], pinfo["playery"], pinfo["grounded"], pinfo["dead"])

	platinfo = info[1]["Items"]
	env.platforms = []
	for plat in platinfo:
		ul = plat['ULx'] , plat['ULy']
		ur = plat['URx'] , plat['URy']
		br = plat['BRx'] , plat['BRy']
		bl = plat['BLx'] , plat['BLy']
		platform = Platform(ul, bl, ur, br, env)
	
	return json.dumps({"result":"success"})
	

@app.route('/reset')
def reset():
	global env
	#print(env.DoIReset())
	return str(env.DoIReset())
	
@app.route('/start')
def start():
	global env
	env = gameEnv(0)
	os.system('"MachineLearning.exe" ')
	return "started game"
	
def index():
	return 'Server Works!'

  

@app.route('/greet')
def say_hello():

	return 'Hello from Server'
