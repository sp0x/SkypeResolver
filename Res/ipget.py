# -*- coding: utf-8 -*-
import socket
import os, os.path
import sys

if len(sys.argv) < 2:
        print('Usage: python skyip.py <skypename>')
        sys.exit();
Username = sys.argv[1]
client = socket.socket( socket.AF_INET, socket.SOCK_STREAM )
client.connect((socket.gethostname(), 133))
client.send(Username)
strResponse = client.recv(1024)
print strResponse
client.close()

#/
##from pprint import *;
#if len(sys.argv) == 1: