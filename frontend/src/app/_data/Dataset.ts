import { FolderFile } from "./FolderFile";

export default class Dataset extends FolderFile {
    constructor(
        name: string = 'Novi izvor podataka',
        public description: string = '',
        public fileId?: string,
        public extension: string = '.csv',
        public isPublic: boolean = false,
        public accessibleByLink: boolean = false,
        dateCreated: Date = new Date(),
        lastUpdated: Date = new Date(),
        public uploaderId: string = '',
        public delimiter: string = ',',

        public columnInfo: ColumnInfo[] = [],
        public rowCount: number = 0,
        public nullRows: number = 0,
        public nullCols: number = 0,
        public isPreProcess : Boolean = false,
        public cMatrix: number[][] = []
    ) {
        super(name, dateCreated, lastUpdated);
    }
}

export class ColumnInfo {
    constructor(
        public columnName: string = '',
        public isNumber: boolean = false,
        public numNulls: number = 0,
        public uniqueValues: string[]=[],
        public uniqueValuesCount: number[]=[],
        public uniqueValuesPercent: number[]=[],
        public median: number=0,
        public mean: number=0,
        public min: number=0,
        public max: number=0,
        public q1: number=0,
        public q3: number=0,
    ) {
        /*if (isNumber)
            this.columnType = ColumnType.numerical;
        else 
            this.columnType = ColumnType.categorical;*/
    }

}

