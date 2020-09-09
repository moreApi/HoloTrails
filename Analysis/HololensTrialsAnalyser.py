import os
import numbers
import numpy as np
import scipy.stats as stats
import matplotlib.pyplot as plt
import math


a1 = "A1"
a2 = "A2"
a3 = "A3"
b2 = "B2"
b3 = "B3"
c1 = "C1"
c3 = "C3"
d1 = "D1"
d2 = "D2"
d3 = "D3"
d4 = "D4"
d5 = "D5"
relevantTrials = [a1,a2,a3,b2,b3,c1,c3,d1,d2,d3,d4,d5]
relTrials = [a1,a2,a3,c3,d1,d2,d3,d4,d5]
relTrials2 = [b2,b3,c1]
m1 = "glow, cursor, grav - clicker - list"
m2 = "glow, cursor - dwell 500 - list"
m3 = "glow, cursor - clicker - list"

 
class Analyser:
    def __init__(self,resultDirectory):
        self.resultDirectory = resultDirectory
        self.logFiles = {}
        self.questionaireFiles = {}
        self.trials = {}
        self.trialsLingerTime = {}
        self.trialTimeToTarget = {}
        self.misses = {}
        self.trialRuns = {}
        self.questionaires = {}
        self.menuTrials = {}
        self.menuPathLength = {}
        self.menuDiscardedTasks = 0
        

        for folder in os.listdir(resultDirectory):
            self.logFiles[folder] = []
            for file in os.listdir(resultDirectory+"\\"+folder):
                if file.startswith("Log"):
                    self.logFiles[folder].append(resultDirectory+"\\"+folder+"\\"+file)
                elif file.startswith("questionnaire"):
                    self.questionaireFiles[folder] = resultDirectory+"\\"+folder+"\\"+file
        
        self.parseQuestionnaires()            
        self.parseLogfiles()
        self.aggregateQuestionnaire()
        
        # calc error rate
        
        

    def parseQuestionnaires(self):
        for folder in self.questionaireFiles:
            with open(self.questionaireFiles[folder],"r") as f:
                comment = False
                
                self.questionaires[folder] = {}
                tmp = self.questionaires[folder]
                
                line = f.readline()
                while not line == "":
                    line = line.strip()
                    if line.startswith("##") or line == "":
                        comment = False
                        #comment or empty line, end of comment section
                    elif comment:
                        if (comment=="menu"):
                            tmp[comment].append(line)
                        else:
                            tmp[comment] += line+"\n"
                    elif line.startswith("#menu"):
                        #use comment functionality to parse menu order, but into an array
                        comment = "menu"
                        tmp[comment] = []
                    elif line.startswith("#"):
                        ident = line[1:3].strip()
                        if ident == "k" or ident == "6" or (len(ident) > 1 and ident[1] == "5" ):
                            comment = ident
                            tmp[ident] = ""
                        else:
                            if ident == "2": #gender
                                tmp[ident] = f.readline().strip().lower()
                            elif ident == "3": #visual aid
                                line = f.readline().strip().lower()
                                if line=="ja":
                                    tmp[ident] = True
                                elif line == "nein":
                                    tmp[ident] = False
                                else:
                                    print (folder+"'s answers for visual aid: "+line+". I don't know what that means.")
                            else: #numerical answer
                                tmp[ident] = float(f.readline()) 
                    
                    
                    line = f.readline()
                  
    def parseLogfiles(self):
        
        for folder in self.logFiles:
            for file in self.logFiles[folder]:
                with open(file,"r") as f:
                                    
                    self._lastStart = 0
                    self._misses = 0
                    self._menuTarget = ("","")
                    self._menuHits = 0;
                    for line in f:
                        splt = line.split(';')
                        if (len(splt) >= 3 and len(splt[1])==2): #is iso Trial 
                            self.parseISO(splt)
                        elif (len(splt) >= 3 and splt[1]=="Menu"):
                            self.parseMenu(splt,folder)                        

    def parseISO(self,splt):
        time = float(splt[0])
        trial = splt[1]
        logType = splt[2]
        note = splt[3]
        
        if (logType == "TrialStart"):
            if (trial not in self.trials.keys()):
                self.trials[trial] = [];
                self.trialsLingerTime[trial] = [];
                self.trialTimeToTarget[trial] = [];
                self.misses[trial] = []
                self.trialRuns[trial] = 0;
                
        elif (logType == "TaskStart"):
            run = note.split(':')
            if int(run[1]) < 4:
                #ignore first runs
                self._lastStart = 0
            else:
                self._lastStart = time
                self._misses = 0;
                self.trialRuns[trial] += 1
                
            
        elif (logType == "Hit"):
            if (not self._lastStart == 0):
                diff = time-self._lastStart
                linger = float(note.strip())
                self.trialsLingerTime[trial].append(linger)
                self.trials[trial].append(diff)
                self.trialTimeToTarget[trial].append(diff-linger)
                self.misses[trial].append(self._misses)
                self._lastStart = 0
                
        elif (logType == "Miss"):
            self._misses += 1
            
    #Menu logs need to be in one file
    def parseMenu(self,splt,folder):
        time = float(splt[0])
        #trial = splt[1]
        logType = splt[2]
        note = splt[3]
        
        if (logType == "TaskStart"):
            self._lastStart = time
            note = note.split('. ')
            note[0] = int(note[0])
            
            if (self._menuTarget):
                self._menuTarget = note
                self._lastStart = time;
                if (self._menuHits < 3):
                    self.menuDiscardedTasks += 1
                self._menuHits = 0;
            else:
                self._menuTarget = "ignore first target"
                
        elif (logType == "Hit"):
            self._menuHits += 1
            if (note == self._menuTarget[1] and self._menuHits >= 3):
                #ignoring broken resets
                diff = time-self._lastStart
                trialNum = int((self._menuTarget[0]-1)/10)
                trial = self.questionaires[folder]["menu"][trialNum]
                if not trial in self.menuTrials:
                    self.menuTrials[trial] = []
                    self.menuPathLength[trial] = []
                self.menuTrials[trial].append(diff)
                self.menuPathLength[trial].append(self._menuHits)
                self._menuTarget = ("","")

    def aggregateQuestionnaire(self):
        self.aggregatedQuestionnaire = {}
        
        for k in self.questionaires["1"].keys():
            agg = []
            for q in self.questionaires:
                agg.append(self.questionaires[q][k])
            
            if isinstance(agg[0], numbers.Number):
                agg = np.mean(agg)
                
            self.aggregatedQuestionnaire[k] = agg
            
        
    def getTrial(self,trial):
        trial = trial.upper()
        if trial in self.trials:
            return self.trials[trial]
        
    def getTrialLinger(self,trial):
        trial = trial.upper()
        if trial in self.trials:
            return self.trialsLingerTime[trial]
        
    def getTrialTtT(self,trial):
        trial = trial.upper()
        if trial in self.trials:
            return self.trialTimeToTarget[trial]
    
    def getTrialMisses(self,trial):
        trial = trial.upper()
        if trial in self.trials:
            return self.misses[trial]
    
    def getTrialData(self,trial):
        """ returns (comp. time, linger, TtT, misses) """
        trial = trial.upper()
        if trial in self.trials:
            return (self.trials[trial],self.trialsLingerTime[trial],self.trialTimeToTarget[trial],self.misses[trial])
        
    def getMenuTrial(self,trial):
        if trial in self.menuTrials:
            return self.menuTrials[trial]
        
    def getMenuPathLength(self,trial):
        if trial in self.menuTrials:
            return self.menuPathLength[trial]
        
    def arrCompare(self,arr1,arr2):
        print (stats.f_oneway(arr1,arr2))
        print ("arr1",np.mean(arr1),stats.normaltest(arr1))
        print ("arr2",np.mean(arr2),stats.normaltest(arr2))
            
    def _compare(self,trial1,trial2,method,trial3=None,fakeFactor=1):
        
        trial1arr = method(self,trial1) * fakeFactor
        trial2arr = method(self,trial2) * fakeFactor
        trial3arr = None
        if trial3:
            trial3arr = method(self,trial3) * fakeFactor
            print (stats.f_oneway(trial1arr,trial2arr,trial3arr))
            print (trial1+" vs. "+trial2, stats.f_oneway(trial1arr,trial2arr))
            print (trial1+" vs. "+trial3, stats.f_oneway(trial1arr,trial3arr))
            print (trial2+" vs. "+trial3, stats.f_oneway(trial2arr,trial3arr))
        
        else:
            print (stats.f_oneway(trial1arr,trial2arr))
            print (trial1,np.mean(trial1arr))#),stats.normaltest(trial1arr))
            print (trial2,np.mean(trial2arr))#,stats.normaltest(trial2arr))
        
    def compare(self,trial1,trial2,trial3=None,fakeFactor=1):
        self._compare(trial1, trial2, Analyser.getTrial, fakeFactor=fakeFactor,trial3=trial3)
        
    def compareLinger(self,trial1,trial2,trial3=None,fakeFactor=1):
        self._compare(trial1, trial2, Analyser.getTrialLinger, fakeFactor=fakeFactor,trial3=trial3)
        
    def compareTtT(self,trial1,trial2,trial3=None,fakeFactor=1): #time to target
        self._compare(trial1, trial2, Analyser.getTrialTtT, fakeFactor=fakeFactor,trial3=trial3)
        
    def isoCompare(self, trial1,trial2,trial3=None):
        print ("completion time")
        self.compare(trial1, trial2,trial3=trial3)
        print ("linger")
        self.compareLinger(trial1, trial2,trial3=trial3)
        print ("to target")
        self.compareTtT(trial1, trial2,trial3=trial3)
        print ("mean errors")
        self._compare(trial1, trial2, Analyser.getTrialMisses,trial3=trial3)
        
    def boxPlotData(self,getMethod,trials,title,xDesc="Conditions",yDesc="Seconds",labels=None):
        data = []
        for x in trials:
            data.append(getMethod(x))
        plt.title(title)
        plt.xlabel(xDesc)
        plt.ylabel(yDesc)
        plt.grid(b=True, which="both", axis="y")
        if not labels:
            labels = trials
        plt.boxplot(data,labels=labels,showmeans=True,meanline=True,showfliers=False)
        
    def barMeanStackedPlotData(self,getMethod,getMethod2,trials,title,xDesc="Conditions",yDesc="Seconds",legend=("Time to target","Linger Time")):
        data1 = []
        data2 = []
        data1std = []
        data2std = []
        for x in trials:
            data1.append(np.mean(getMethod(x)))
            data1std.append(np.std(getMethod(x)))
            data2.append(np.mean(getMethod2(x)))
            data2std.append(np.std(getMethod2(x)))
            
        plt.title(title)
        plt.ylabel(yDesc)
        plt.xlabel(xDesc)
        plt.grid(b=True, which="both", axis="y")
        
        N = len(trials)
        ind = np.arange(N)
        p1 = plt.bar(ind, data1, color='#d62728')
        p2 = plt.bar(ind, data2, bottom=data1)
        plt.xticks(ind, trials)
        plt.yticks(np.arange(0, 4, 0.5))
        plt.legend((p1[0], p2[0]), legend)
        
        
    def barMeanPlotData(self,getMethod,trials,title,yDesc="Errors",xDesc="Conditions",labels=None,stdErr=False):
        if not labels:
            labels = trials
            
        data1 = []
        data1Std = []
        
        
        for x in trials:
            tmp = getMethod(x)
            data1.append(np.mean(tmp))
            data1Std.append(np.std(tmp))
            
        if not stdErr:
            data1Std = None
            
        plt.title(title)
        plt.ylabel(yDesc)
        plt.xlabel(xDesc)
        plt.grid(b=True, which="both", axis="y")
        
        N = len(trials)
        ind = np.arange(N)
        plt.bar(ind, data1, color='#d62728', yerr=data1Std)
        plt.xticks(ind, labels)
        #plt.yticks(np.arange(0, 4, 0.5))
        
        
#...............

    def printIsoToTableRow(self,trial):
        trialData = list(self.getTrialData(trial))
        trialData.append(np.std(trialData[0])) #add std
        trialMeanData = list(map(np.mean,trialData)) #calc mean
        #mean comp. time, linger, TtT, misses, std
        tmd = list(map(lambda x: str(x)[0:5], trialMeanData))
        
        print(trial+" & "+tmd[0]+" & "+tmd[4]+" & "+tmd[1]+" & "+tmd[2]+" & "+tmd[3]+" & "+"""\\\\""")
        
    def printMenuToTableRow(self):
        trials = [m1,m2,m3]
        ids = ["M1","M2","M3"]
        comp = list(map(self.getMenuTrial,trials))
        compMean = list(map(np.mean,comp))
        std = list(map(np.std,comp))
        pathLen = list(map(lambda x: np.mean(self.getMenuPathLength(x)),trials))
        for i in range(len(trials)):
            print (str(ids[i])+" & "+str(compMean[i])[0:4]+"s & "+str(std[i])[0:4]+"s & "+str(pathLen[i])[0:4]+" & "+"\\\\")
        


a = Analyser("..\\Results")
data3 = a.getTrialData(a3)
data2 = a.getTrialData(a2)
data1 = a.getTrialData(a1)

def diffId(vec1,vec2,size):
    len = ((vec1[0]-vec2[0])**2+ (vec1[1]-vec2[1])**2+(vec1[2]-vec2[2])**2)**(0.5)
    
    return (len,math.log((len + size) / size, 2))
    
    
    


