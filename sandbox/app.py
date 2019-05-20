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
	def __init__(self, x, y, g, d, i, env):
		self.x = x
		self.y = y
		self.grounded = g
		self.id = i
		self.env = env
		env.addPlayer(self, i)
		self.distanceToJump = 0.2
		self.dead = d
		
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
		#return 1,0
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
	def __init__(self, counter, limit, numbPlayers):
		print("Reinitialized game environment")
		self.players = {}
		self.platforms = []
		self.counter = counter
		self.limit = limit
		self.numbPlayers = numbPlayers
		
	def addPlayer(self, player, id):
		self.players[id] = (player)

	def addPlatform(self, platform):
		self.platforms.append(platform)
	
	def AllPlayersDead(self):
		if len(self.players) < self.numbPlayers:
			return False
		ret = True
		for k, p in self.players.items():
			if not p.dead:
				ret = False
		return ret
		
	def DoIReset(self):
		return self.AllPlayersDead()

env = gameEnv(0, 5, 20)

@app.route('/hello')
def hello():
	global env
	env = gameEnv(env.counter + 1, env.limit, env.numbPlayers)
	return "Hello mistress sexy bunny"
	
@app.route('/nplayers')
def nplayers():
	global env
	return json.dumps({"N":env.numbPlayers})
	
@app.route('/getmove', methods=['POST'])
def getmove():
	'''Put the input here, direction = 1 means right, direction = -1 means left
	jump = 0 means no jump, jump =1 means jump'''
	global env
	info = request.get_json()
	id = info["id"]
	mv = env.players[id].nextMove()
	print(mv)
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
	pinfo =  info[0]["Items"]
	print(info)
	env.players = {}
	for p in pinfo:
		player = Player(p["playerx"], p["playery"], p["grounded"], p["dead"], int(p["id"]), env)
	#print(env.players)
	platinfo = info[1]["Items"]
	env.platforms = []
	for plat in platinfo:
		ul = plat['ULx'] , plat['ULy']
		ur = plat['URx'] , plat['URy']
		br = plat['BRx'] , plat['BRy']
		bl = plat['BLx'] , plat['BLy']
		platform = Platform(ul, bl, ur, br, env)
	if env.DoIReset():
		os.system('taskkill /f /t /im MachineLearning.exe')
		if env.counter < env.limit:		
			os.system('"MachineLearning.exe"')
	return json.dumps({"result":"success"})
	

@app.route('/reset')
def reset():
	global env
	#print(env.DoIReset())
	return str(env.DoIReset())
	
@app.route('/start', methods=['POST'])
def start():
	global env
	info = request.get_json()
	#print(info)
	env = gameEnv(0, int(info["limit"]), int(info["numbPlayers"]))
	os.system('"MachineLearning.exe" ')
	return "started game"
	
def index():
	return 'Server Works!'

  

@app.route('/greet')
def say_hello():

	return 'Hello from Server'
