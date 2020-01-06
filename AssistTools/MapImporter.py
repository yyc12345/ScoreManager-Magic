import os
import hashlib

names = []
files = []

def getSha256(filname):
    ''' calculate file sha256 '''
    with open(filname, "rb") as f:
        sha256obj = hashlib.sha256()
        sha256obj.update(f.read())
        hash_value = sha256obj.hexdigest()
        return hash_value

def process(folder):
    global files
    for i in os.listdir(folder):
        if os.path.isdir(folder + '\\' + i):
            # folder
            process(folder + '\\' + i)
        else:
            # file
            cache = i.lower()
            if cache[-10:] == '.level.nmo':
                filename = i[0:-10]
            elif cache[-4:] == ".nmo":
                filename = i[0:-4]
            else:
                continue
            names.append(filename)
            files.append(folder + '\\' + i)

prefix = input("Input prefix: ")
folder = input('Input folder without slash: ')

process(folder)

fs=open('result.sql', 'w', encoding='utf-8')
fs.write('INSERT map (sm_name, sm_i18n, sm_hash) VALUES')
fs.write('\n')
for i in range(len(files)):
    fs.write("('{}', '', '{}'),".format(prefix + names[i], getSha256(files[i])))
    fs.write('\n')

fs.write('\n;')
fs.close()