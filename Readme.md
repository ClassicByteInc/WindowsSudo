
# WindowsSudo

һ���� Windows ƽ̨��ģ�� Linux `sudo` ��Ϊ�������й��ߣ�֧���Թ���ԱȨ�����п�ִ���ļ��� shell �������Ȩ������

## ����

- ֧���Թ���ԱȨ�����������ִ���ļ����� `.exe`����
- ֧���Թ���ԱȨ��ִ�� shell ��������Զ����� PowerShell �� CMD����
- �Զ���Ⲣʹ�õ�ǰ���û�Ĭ�� shell��
- ֧�� .NET 9��AOT �����������ٶȿ졣

## ��װ

ʹ��```dotnet```�����й��߰�װ
```
dotnet tool install sudo
```

�����ڸòֿ��[Release](https://github.com/ClassicByteInc/WindowsSudo/releases)���ҵ�

## ʹ�÷���

### �Թ���ԱȨ�����п�ִ���ļ�
# WindowsSudo

һ���� Windows ƽ̨��ģ�� Linux `sudo` ��Ϊ�������й��ߣ�֧���Թ���ԱȨ�����п�ִ���ļ��� shell �������Ȩ������

## ����

- ֧���Թ���ԱȨ�����������ִ���ļ����� `.exe`����
- ֧���Թ���ԱȨ��ִ�� shell ��������Զ����� PowerShell �� CMD����
- �Զ���Ⲣʹ�õ�ǰ���û�Ĭ�� shell��
- ֧�� .NET 9��AOT �����������ٶȿ졣

## ��װ

ʹ��```dotnet```�����й��߰�װ
### �Թ���ԱȨ��ִ�� shell ����

- PowerShell �£�
- CMD �£�
### �鿴����
## ����Ĭ�� Shell

��ͨ���޸��û�Ŀ¼�� `.sudo/DefaultShell.cfg` �ļ���ָ��Ĭ�� shell ·����

## ����

- .NET 9 SDK
- Windows 10/11

## ���

MIT License

---

> ����Ŀ�����Դ�� Linux �� sudo��ּ������ Windows ����������Ȩ���顣

## ʹ�÷���

### �Թ���ԱȨ�����п�ִ���ļ�

```
sudo notepad.exe C:\Windows\System32\drivers\etc\hosts
```

### �Թ���ԱȨ��ִ�� shell ����

- PowerShell �£�
```
  sudo Copy-Item "C:\a.txt" "D:\b.txt"
```
- CMD �£�
```
  sudo copy "C:\a.txt" "D:\b.txt"
```

### �鿴����

```
sudo

```

## ����Ĭ�� Shell

��ͨ���޸��û�Ŀ¼�� `.sudo/DefaultShell.cfg` �ļ���ָ��Ĭ�� shell ·����

## ����

- .NET 9 SDK
- Windows 10/11

## ���

MIT License

---

> ����Ŀ�����Դ�� Linux �� sudo��ּ������ Windows ����������Ȩ���顣