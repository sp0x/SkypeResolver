print('****************************************************************************');
print('Skype IP lookup deamon');
print('****************************************************************************');

# This deamon listen unix socket for username string and then make RefreshProfile() for username.
# If username online in Skypekit debug log will be string with IP adress.
#
# Put this script in skypekit-sdk_runtime-3.7.0/examples/python/tutorial and run there.
#
# You will need to launch the SkypeKit runtime before running this deamon.



#----------------------------------------------------------------------------------
# Importing necessary libraries. Note that you will need to set the keyFileName value
# in the keypair.py file.

import sys;
import socket
import os, os.path
import time
import keypair;
import stat
from time import sleep;
from pprint import *;
import re
import glob


#------------------------------------------------------------------------
# Put here path to debug log that is currently written by skypekit runtime
# you need run "skypekit -d logname" before edit this
# also you can parse log by youself like "tail -F /some/folder/logname | grep -a noticing" and use client.py only for send username into socket

sys.path.append(keypair.distroRoot + '/ipc/python');
sys.path.append(keypair.distroRoot + '/interfaces/skype/python');

try:
        import Skype;
except ImportError:
  raise SystemExit('Program requires Skype and skypekit modules');

#----------------------------------------------------------------------------------
# Taking skypename and password arguments from command-line.

if len(sys.argv) != 3:
        print('Usage: python vcard_socket.py <skypename> <password>');
        sys.exit();

accountName = sys.argv[1];
accountPsw  = sys.argv[2];
loggedIn        = False;
myhost = "127.0.0.1"# socket.gethostname()
myport = 133



#----------------------------------------------------------------------------------
# Creating our main Skype object

try:
        MySkype = Skype.GetSkype(keypair.keyFileName);
        MySkype.Start();
except Exception:
        raise SystemExit('Unable to create Skype instance');

def AccountOnChange (self, property_name):
  global loggedIn;
  if property_name == 'status':
    print ('Login sequence: ' + self.status);
    if self.status == 'LOGGED_IN':
      loggedIn = True;
    if self.status == 'LOGGED_OUT':
      loggedIn = False;


Skype.Account.OnPropertyChange = AccountOnChange;
account = MySkype.GetAccount(accountName);
account.LoginWithPassword(accountPsw, False, False);
while loggedIn == False:
        sleep(1);



print "Logged, opening socket..."
sx = socket.socket( socket.AF_INET, socket.SOCK_STREAM )
sx.bind((myhost, myport))
print "LISTENING on " + str(myhost) + ":" + str(myport)

#----------------------------------------------------------------------------------
# Main cycle.
#
while True:
        sx.listen(1)
        conn, addr = sx.accept()

        targetUsr = conn.recv( 1024 )
        if not targetUsr:
         break
        print (str(addr) + ' Looking for ' + targetUsr + '...')
        MySkype.GetContact(targetUsr).RefreshProfile()
        time.sleep(3)

        newlog = max(glob.iglob('*.log'), key=os.path.getctime)
        print 'Using the log: ' + newlog

        File = open(newlog,'rb').readlines()
        finds = []
        for matches in File:
                found = re.findall('.*noticing.{0}.0.*-r(.*?)-l(.*?:[0-9]*?)[^0-9].*'.format(targetUsr), matches)
                if len(found)>0:
                        finds.append('%s#%s' % (found[0][0], found[0][1]) )
        finds = os.linesep.join(list(set(finds)))
        conn.send(finds)