import * as React from "react";
import { Route } from "react-router";
import DefaultLayout from "./layouts/DefaultLayout";
import Home from "./pages/Home";
import Configuration from "./pages/Configuration";

import "./custom.css";

export default () => (
  <DefaultLayout>
    <Route exact path="/" component={Home} />
    <Route path="/configuration" component={Configuration} />
  </DefaultLayout>
);
