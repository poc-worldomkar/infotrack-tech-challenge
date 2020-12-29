import { Typography } from "@material-ui/core";
import * as React from "react";
import { connect } from "react-redux";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../store";
import * as CounterStore from "../store/Counter";

type CounterProps = CounterStore.CounterState &
  typeof CounterStore.actionCreators &
  RouteComponentProps<{}>;

class Counter extends React.PureComponent<CounterProps> {
  public render() {
    return (
      <>
        <Typography variant="h6">Configuration</Typography>
      </>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.counter,
  CounterStore.actionCreators
)(Counter);
