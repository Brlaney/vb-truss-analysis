Option Strict On
Option Infer On
Imports BriefFiniteElementNet
Imports BriefFiniteElementNet.Elements

Module Program
    Sub Main()
        Dim Model = New Model()

        Dim n1 = New Node(0, 0, 0)
        Dim n2 = New Node(1, 0, 0)

        n1.Constraints = Constraints.Fixed
        n2.Constraints = Constraints.Released

        Model.Nodes.Add(n1)
        Model.Nodes.Add(n2)

        Dim elm = New BarElement(n1, n2)
        Model.Elements.Add(elm)

        elm.Section = New Sections.UniformParametric1DSection(a:=0.01,
                                                              iy:=0.0000083,
                                                              iz:=0.0000083,
                                                              j:=0.000166)

        elm.Material = Materials.UniformIsotropicMaterial.CreateFromShearPoisson(shearModulus:=210000000000.0,
                                                                                 poissonModulus:=0.3)

        Dim Load = New NodalLoad()
        Dim frc = New Force()

        frc.Fz = 1000 '1kN force In Z direction
        Load.Force = frc

        n2.Loads.Add(Load)

        Model.Solve()

        Dim d2 = n2.GetNodalDisplacement()

        Dim v11 = d2.DZ.ToString("0.000000")
        Dim v12 = (d2.DZ * 1000).ToString("0.000000")
        Dim v21 = d2.RY.ToString("0.000000")
        Dim v22 = (d2.RY * 180.0 / Math.PI).ToString("0.000000")

        Dim OutputString As New Text.StringBuilder
        With OutputString
            .AppendLine($"Nodal displacement along the z-axis:   {v11} m    (Or: {v12} mm )")
            .AppendLine($"Nodal rotation along the y-axis:       {v11} rad  (Or: {v22} deg)")
        End With

        Console.WriteLine(OutputString.ToString)
    End Sub
End Module
