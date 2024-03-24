from app import init_app
from os import path
import sys

sys.path.append(path.abspath("D:\GitHubDesktop - new\Praca-Inzynierka"))
sys.path.append(path.abspath("D:\GitHubDesktop - new\Praca-Inzynierka\RecommendationSystem"))

server = init_app()
