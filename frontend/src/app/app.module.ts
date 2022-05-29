import { NgModule, CUSTOM_ELEMENTS_SCHEMA, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { MatSliderModule } from '@angular/material/slider';
import { MatIconModule } from '@angular/material/icon';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { NgChartsModule } from 'ng2-charts';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
// Modules and modals
import { Configuration } from './_services/configuration.service';
import { MaterialModule } from './material.module';
import { LoginModalComponent } from './_modals/login-modal/login-modal.component';
import { RegisterModalComponent } from './_modals/register-modal/register-modal.component';
import { AlertDialogComponent } from './_modals/alert-dialog/alert-dialog.component';
import { YesNoDialogComponent } from './_modals/yes-no-dialog/yes-no-dialog.component';
import { EncodingDialogComponent } from './_modals/encoding-dialog/encoding-dialog.component';
import { MissingvaluesDialogComponent } from './_modals/missingvalues-dialog/missingvalues-dialog.component';
import { SaveExperimentDialogComponent } from './_modals/save-experiment-dialog/save-experiment-dialog.component';
import { UpdateExperimentDialogComponent } from './_modals/update-experiment-dialog/update-experiment-dialog.component';
// Pages
import { HomeComponent } from './_pages/home/home.component';
import { ProfileComponent } from './_pages/profile/profile.component';
import { ExperimentComponent } from './_pages/experiment/experiment.component';
import { PlaygroundComponent } from './_pages/playground/playground.component';
import { ArchiveComponent } from './_pages/archive/archive.component';
// Charts
import { ScatterchartComponent } from './_elements/_charts/scatterchart/scatterchart.component';
import { BarchartComponent } from './_elements/_charts/barchart/barchart.component';
import { PieChartComponent } from './_elements/_charts/pie-chart/pie-chart.component';
import { BoxPlotComponent } from './_elements/_charts/box-plot/box-plot.component';
import {LineChartComponent} from './_elements/_charts/line-chart/line-chart.component';
// Elements
import { NavbarComponent } from './_elements/navbar/navbar.component';
import { NotificationsComponent } from './_elements/notifications/notifications.component';
import { DatatableComponent } from './_elements/datatable/datatable.component';
import { ReactiveBackgroundComponent } from './_elements/reactive-background/reactive-background.component';
import { LoadingComponent } from './_elements/loading/loading.component';
import { GraphComponent } from './_elements/graph/graph.component';
import { GradientBackgroundComponent } from './_elements/gradient-background/gradient-background.component';
import { PlaylistComponent } from './_elements/playlist/playlist.component';
import { FormDatasetComponent } from './_elements/form-dataset/form-dataset.component';
import { FormModelComponent } from './_elements/form-model/form-model.component';
import { ColumnTableComponent } from './_elements/column-table/column-table.component';
import { FolderComponent } from './_elements/folder/folder.component';
import { TestComponent } from './_pages/test/test.component';
import { DoughnutChartComponent } from './_elements/_charts/doughnut-chart/doughnut-chart.component';
import { HeatmapComponent } from './_elements/_charts/heatmap/heatmap.component';
import { HeatMapAllModule } from '@syncfusion/ej2-angular-heatmap';
import { MetricViewComponent } from './_elements/metric-view/metric-view.component';
import { SpinnerComponent } from './_elements/spinner/spinner.component';

export function initializeApp(appConfig: Configuration) {
  return () => appConfig.load();
}
@NgModule({
  declarations: [
    AppComponent,
    LoginModalComponent,
    RegisterModalComponent,
    HomeComponent,
    NavbarComponent,
    ProfileComponent,
    ScatterchartComponent,
    BarchartComponent,
    NotificationsComponent,
    DatatableComponent,
    ReactiveBackgroundComponent,
    ExperimentComponent,
    LoadingComponent,
    AlertDialogComponent,
    GraphComponent,
    YesNoDialogComponent,
    PlaygroundComponent,
    GradientBackgroundComponent,
    PlaylistComponent,
    ArchiveComponent,
    FormDatasetComponent,
    FormModelComponent,
    ColumnTableComponent,
    PieChartComponent,
    BoxPlotComponent,
    FolderComponent,
    EncodingDialogComponent,
    MissingvaluesDialogComponent,
    TestComponent,
    DoughnutChartComponent,
    HeatmapComponent,
    MetricViewComponent,
    LineChartComponent,
    SaveExperimentDialogComponent,
    SpinnerComponent,
    UpdateExperimentDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    NgbModule,
    BrowserAnimationsModule,
    MaterialModule,
    ReactiveFormsModule,
    MatSliderModule,
    MatIconModule,
    NgChartsModule,
    Ng2SearchPipeModule,
    HeatMapAllModule
  ],
  providers: [
    Configuration,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [Configuration], multi: true
    }
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [AlertDialogComponent]
})
export class AppModule { }
