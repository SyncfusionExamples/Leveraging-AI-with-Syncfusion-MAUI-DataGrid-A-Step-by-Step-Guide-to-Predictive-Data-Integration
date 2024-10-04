﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnamolyPrediction.Model
{
    public class MachineData
    {
        private string _machineID;

        private int _temperature;
        private int _pressure;
        private int voltage;
        private int _motorSpeed;
        private int _productionRate;
        private string _anomolyDescription;
        public string MachineID
        {
            get { return _machineID; }
            set { _machineID = value; }
        }

        public int Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public int Pressure
        {
            get { return _pressure; }
            set { _pressure = value; }
        }
        public int Voltage
        {
            get { return voltage; }
            set { voltage = value; }
        }
        public int MotorSpeed
        {
            get { return _motorSpeed; }
            set { _motorSpeed = value; }
        }
        public int ProductionRate { get { return _productionRate; } set { _productionRate = value; } }
        public string AnomalyDescription { get { return _anomolyDescription; } set { _anomolyDescription = value; } }

        public MachineData(string MachineID, int temp, int pressure, int volt, int motarSpeed, int productionRate, string des)
        {
            this._machineID = MachineID;
            this._temperature = temp;
            this._pressure = pressure;
            this.voltage = volt;
            this._motorSpeed = motarSpeed;
            this._productionRate = productionRate;
            this._anomolyDescription = des;
        }
    }
}
