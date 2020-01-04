import requests
import urllib

getheaders = {'Host': 'tieba.baidu.com',
           'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36'}

getheaders2 = {'Host': 'himg.baidu.com',
           'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36'}

def strmid(head, tail, target):
    index1 = target.find(head)
    if index1 == -1:
        return ""
    target = target[index1+len(head):]
    index1 = target.find(tail)
    if index1 == -1:
        return ""
    return target[0:index1]

while True:
    print("========================================")
    smmName = input("Input SMM name: ")
    tiebaName = input("Input tieba name: ")

    encodecName = urllib.parse.quote(tiebaName.encode('gb2312'))
    portal = requests.get("http://tieba.baidu.com/i/data/panel?un=" + encodecName, headers = getheaders)
    if portal.status_code != 200:
        print("Fail to get")
        continue

    imageUrl = strmid("<img class=\"head_img\" src=\"", "\" >", portal.text)
    imageName = imageUrl.split('/')[6]
    image = requests.get("http://himg.baidu.com/sys/portraitl/item/" + imageName, headers = getheaders2)
    if image.status_code != 200:
        print("Fail to get")
        continue
    fs=open(smmName + ".jpg", 'wb')
    fs.write(image.content)
    fs.close()

    print("Done")
