#define R 9
#define G 3
#define Y 5
#define B 6
#define W 10


void setState(byte cmd,byte data1)//设定Arduino相应的输出端状态。0为低电平，1为高电平。
{
 if(data1==0)
  {
    byte pin=cmd&0x0f;
    digitalWrite(pin,0);
  }
  else if(data1==1)
 {
  byte pin=cmd&0x0f;
  digitalWrite(pin,1);
 }
}

void setNum(byte cmd,byte data1,byte data2)//设定灯的亮度
{
  byte pin=cmd&0x0f;
  byte num=data1+(data2<<7);
  analogWrite(pin,num);
}

void readState(byte cmd)//读取引脚状态
{
  byte pin=cmd&0x0f;
  byte a=digitalRead(pin);
  Serial.write(a);
}

void readAD(byte cmd)
{
  byte pin=cmd&0x0f;
  int num=analogRead(pin);
  int temp=(num&0x380); // 7-9bits
  byte a2=(temp>>7);
  byte a1=(num&0x7f);  //0-6bits
  byte a=0xE0;
  Serial.write(a);
  Serial.write(a1);
  Serial.write(a2);
}


void returnId()
{
  Serial.write(0xF9);
  Serial.write(0x50);
  Serial.write(0x3C);
}

void setup() {
  Serial.begin(9600);
  pinMode(R, OUTPUT);
  pinMode(G, OUTPUT);
  pinMode(Y, OUTPUT);
  pinMode(B, OUTPUT);
  pinMode(W, OUTPUT);
}

byte data[3];
int index=0;
byte aa=0;
byte bb=0;
void loop() {
    if(aa)
    {
        int num=analogRead(0);
        int temp=(num&0x380); // 7-9bits
        byte a2=(temp>>7);
        byte a1=(num&0x7f);  //0-6bits
        byte a=0xE0;
        Serial.write(a);
        Serial.write(a1);
        Serial.write(a2);

        num=analogRead(1);
        temp=(num&0x380); // 7-9bits
        a2=(temp>>7);
        a1=(num&0x7f);  //0-6bits
        a=0xE1;
        Serial.write(a);
        Serial.write(a1);
        Serial.write(a2);
        delay(2000);
    }
    while(bb)
    {
      byte rxdata1=Serial.read();
      byte rxdata2=Serial.read();
      byte rxdata3=Serial.read();
      byte pin=rxdata1&0x0f;
      if(pin==0||pin==1||pin==2)
      {
        pin=3;
      }else
      if(pin==3||pin==4||pin==5)
      {
        pin=5;
      }else
      if(pin==6||pin==7||pin==8)
      {
        pin=6;
      }else
      if(pin==9||pin==10||pin==11){
        pin=9;
      }else
      {
        pin=10;
      }
      byte num=rxdata2+((rxdata3&0x01)<<7);
      analogWrite(pin,num);
     
      delay(10);
    }
    if(Serial.available()>0)
    {
      byte rxdata=Serial.read();
      data[index]=rxdata;
      if((rxdata&0x80)!=0)//收到命令
      {
        if(index!=0)
        {
          index=0;
          Serial.write(0xFA);
          Serial.write(0x7F);
          Serial.write(0x7F);//接收到无效信息
        }
        data[0]=rxdata;
        index++;
      }else if(index!=0)
      {
        if(index<3)
        {
           data[index]=rxdata;
           index++;
        }
        if(index==3)
        {
          byte cmd=data[0];//前4位为命令，后四位为引脚号
          byte data1=data[1];
          byte data2=data[2];
          if((cmd&0xf0)==0x90)
          {
            setState(cmd,data1);
          }else
          if((cmd&0xf0)==0xD0)
          {
            setNum(cmd,data1,data2);
          }else
          if((cmd&0xf0)==0xC0)
          {
            readState((cmd));
          }else
          if((cmd&0xf0)==0xE0)
          {
            aa=1;
          }else
          if((cmd&0xf0)==0xA0)
          {
            aa=0;
          }else
          if((cmd&0xF9)==0xF9)
          {
           returnId(); 
          }else
          if((cmd&0xF8)==0xF8)
          {
            bb=1;
          }else
          if((cmd&0xF7)==0xF7)
          {
            bb=0;
          }
          index=0;
        }
      }
    }
    
    delay(10);
}
