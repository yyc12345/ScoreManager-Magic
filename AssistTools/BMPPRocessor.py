import os
import hashlib
import shutil
import random

bmpFolder = input('BMP folder without slash: ')
nmoFolder = input('NMO folder without slash: ')

def getSha256(filname):
    ''' calculate file sha256 '''
    with open(filname, "rb") as f:
        sha256obj = hashlib.sha256()
        sha256obj.update(f.read())
        hash_value = sha256obj.hexdigest()
        return hash_value

nmoFiles = []
for i in os.listdir(nmoFolder):
    if i[-3:] == "nmo" or i[-3:] == "NMO":
        nmoFiles.append(i)
    
for i in nmoFiles:
    shutil.copyfile(nmoFolder + "\\" + i, bmpFolder + "\\Map.nmo")
    print(i)
    print(getSha256(nmoFolder + "\\" + i))
    print("Press S {} times".format(random.randint(0,11)))
    os.system(bmpFolder + "\\Launch.bat")
    input("===================================================")

print("Done")
