import { Component, Inject } from '@angular/core';
import { Headers, Http, RequestOptions } from '@angular/http';
import { Observable } from "rxjs/Observable";
import 'rxjs/add/observable/forkJoin';

@Component({
    selector: 'vehicles',
    templateUrl: './vehicles.component.html',
    styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent {
    public vehicles: Vehicle[];
    public selectedVehicle: Vehicle | undefined;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.refreshData();
    }

    async refreshData() {
        this.http.get(this.baseUrl + 'api/vehicles').subscribe(result => {
            let vehicleList = [];

            for (let stud of result.json() as Vehicle[]) {

                let vehicle = new Vehicle();
                vehicle.id = stud.id;
                vehicle.brand = stud.brand;
                vehicle.model = stud.model;
                vehicle.price = stud.price;
                vehicle.hasChanges = false;
                vehicleList.push(vehicle);
            }

            console.log("ok");

            this.vehicles = vehicleList;

            this.selectVehicle();
        }, error => console.error(error));
    }


    selectVehicle(): void {

        this.selectedVehicle = undefined;

        for (let stud of this.vehicles) {
            if (stud.deleted == false) {
                this.selectedVehicle = stud;
                break;
            }

        }
    }


    async putData(): Promise<void> {
        let headers = new Headers({ 'Content-Type': 'application/json' });

        let serverCalls = [];

        for (let vehicle of this.vehicles) {
            if (vehicle.hasChanges == true || vehicle.deleted) {

                let json = JSON.stringify(vehicle.toJSON());

                if (!vehicle.id) { //create
                    if (!vehicle.deleted) {
                        let call = this.http.put(this.baseUrl + 'api/vehicles', json, { headers: headers });
                        serverCalls.push(call);
                    }
                }
                else {
                    if (vehicle.deleted) {
                        let url = this.baseUrl + 'api/vehicles?id=' + vehicle.id;
                        let call = this.http.delete(url, { headers: headers });
                        serverCalls.push(call);
                    }
                    else {
                        let call = this.http.post(this.baseUrl + 'api/vehicles', json, { headers: headers });
                        serverCalls.push(call);
                    }

                }
            }
        }
        Observable.forkJoin(serverCalls)
            .subscribe(data => {
                this.refreshData();
            }, error => console.error(error));


    }

    onSelect(vehicle: Vehicle): void {

        if (vehicle.deleted == false) {
            this.selectedVehicle = vehicle;
        }
    }

    addNewVehicle(): void {
        this.selectedVehicle = new Vehicle();
        this.selectedVehicle.hasChanges = true;
        this.vehicles.push(this.selectedVehicle);
    }

    async saveChanges(): Promise<void> {
        await this.putData();
        //console.log("update completed");
        //await this.refreshData();
    }

    delete(vehicle: Vehicle): void {
        vehicle.deleted = true;
        this.selectVehicle();
    }
}

class Vehicle {
    id: number;

    private _brand: string = "";
    private _model: string = "";
    private _price: string = "";
    public hasChanges: boolean;
    public deleted: boolean = false;


    //BRAND
    get brand(): string {
        return this._brand;
    }
    set brand(n: string) {
        this._brand = n;
        this.hasChanges = true;
        console.log("set brand");
    }

    //MODEL
    get model(): string {
        return this._model;
    }
    set model(n: string) {
        this._model = n;
        this.hasChanges = true;
        console.log("set model");
    }

    //PRICE
    get price(): string {
        return this._price;
    }
    set price(n: string) {
        this._price = n;
        this.hasChanges = true;
        console.log("set price");
    }


    public toJSON() {
        return {
            id: this.id,
            brand: this._brand,
            model: this._model,
            price: this._price,
        };
    };
}
